module SessionsRepository

open Models
open Entities
open System
open System.Net
open System.Configuration
open MySql.Data.MySqlClient
open Dapper
open System.Data.SqlClient

//TODO Use Result DU 
//TODO common code with other repo
//TODO open Dapper.Contrib.Extensions

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
let getConnection() = new MySqlConnection(connectionString)

let sessionSummarySql = """SELECT 
        `s`.`id` AS `id`,
        `s`.`title` AS `title`,
        `s`.`status` AS `status`,
        `s`.`speakerId` AS `speakerId`,
        `s`.`date` AS `date`,
        `s`.`dateAdded` AS `dateAdded`,
        `sp`.`forename` AS `speakerForename`,
        `sp`.`surname` AS `speakerSurname`,
        `sp`.`imageUrl` AS `speakerImageUrl`,
        `sp`.`rating` AS `speakerRating`,
        `s`.`adminId` AS `adminId`,
        `a`.`forename` AS `adminForename`,
        `a`.`surname` AS `adminSurname`,
        `a`.`imageUrl` AS `adminImageUrl`,
        `s`.`threadId` AS `threadId`
    FROM
        ((`sessions` `s`
        LEFT JOIN `profiles` `sp` ON ((`sp`.`id` = `s`.`speakerId`)))
        LEFT JOIN `profiles` `a` ON ((`a`.`id` = `s`.`adminId`)))"""

let entityToSession (entity : SessionEntity) : Session =
    { Id = entity.Id
      Title = entity.Title
      Status = entity.Status
      Date = Option.ofNullable entity.Date
      SpeakerId = entity.SpeakerId
      AdminId = entity.AdminId
      ThreadId = entity.ThreadId
      DateAdded = Some entity.DateAdded }

let sessionToEntity (session : Session) : SessionEntity =
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date = session.Date |> Option.toNullable
      SpeakerId = session.SpeakerId
      AdminId = session.AdminId
      ThreadId = session.ThreadId
      DateAdded = 
        match session.DateAdded with
        | None -> DateTime.UtcNow
        | Some date -> date }

let getSessionSummaries() = 
    use connection = getConnection()
    connection.Open()

    let result = connection.Query<SessionSummaryEntity>(sessionSummarySql)

    connection.Close()
    result

let getSessions() =
    use connection = getConnection()
    connection.Open()

    let result = connection.Query<SessionEntity>("SELECT * FROM sessions")

    connection.Close()
    result |> Seq.map entityToSession

type SessionSelectArgs = 
    { SessionId : Guid }

let getSessionSummary (id : Guid) =
    use connection = getConnection()
    connection.Open()

    let args = { SessionId = id }
    let sessions = connection.Query<SessionSummaryEntity>(sessionSummarySql + " WHERE `s`.`id` = @SessionId", args)
    let result = 
        if Seq.isEmpty sessions then None
        else Some(Seq.head sessions)

    connection.Close()
    result

let getSession (id : Guid) = 
    use connection = getConnection()
    connection.Open()

    let args = { SessionId = id }
    let sessions = connection.Query<SessionEntity>("SELECT * FROM sessions WHERE id = @SessionId", args)
    let result = 
        if Seq.isEmpty sessions then None
        else Some(entityToSession (Seq.head sessions))

    connection.Close()
    result

let createSession (session : Session) = 
    use connection = getConnection()
    connection.Open()
    use transaction = connection.BeginTransaction()

    try

        let args = { session with Id = Guid.NewGuid() } |> sessionToEntity

        connection.Execute("insert into sessions(id, title, status, date, speakerId, adminId, threadId, dateAdded) values 
            (@Id, @Title, @Status, @Date, @SpeakerId, @AdminId, @ThreadId, @DateAdded)", args) |> ignore

        transaction.Commit()
        connection.Close()

        Success args.Id

    with 
        | :? SqlException as ex -> 
            transaction.Rollback()
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.BadRequest; Message = ex.Message}
        | _-> 
            transaction.Rollback()
            connection.Close()
            Failure { HttpStatus = HttpStatusCode.InternalServerError; Message = "Internal Server Error"}
