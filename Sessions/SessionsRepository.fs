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

let emptyGuid = new Guid("00000000000000000000000000000000")

let entityToSession (entity : SessionSummaryEntity) : Session =
    let speaker =
        { Id = entity.SpeakerId
          Forename = entity.SpeakerForename
          Surname = entity.SpeakerSurname
          Rating = enum<Rating> entity.SpeakerRating
          ImageUri = entity.SpeakerImageUrl }
    let admin =
        if entity.AdminId = emptyGuid then None
        else Some { AdminSummary.Id = entity.AdminId
                    Forename = entity.AdminForename
                    Surname = entity.AdminSurname
                    ImageUri = entity.AdminImageUrl }
    { Id = entity.Id
      Title = entity.Title
      Status = entity.Status
      Date = Option.ofNullable entity.Date
      DateAdded = entity.DateAdded
      ThreadId = entity.ThreadId
      Speaker = speaker
      Admin = admin }

let sessionToEntity (session : NewSession) : SessionEntity =
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

let getSessions() = 
    use connection = getConnection()
    connection.Open()

    let result = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries")

    connection.Close()
    result |> Seq.map entityToSession

type SessionSelectArgs = 
    { SessionId : Guid }

let getSession (id : Guid) =
    use connection = getConnection()
    connection.Open()

    let args = { SessionId = id }
    let sessions = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries WHERE id = @SessionId", args)
    let result = 
        if Seq.isEmpty sessions then None
        else sessions |> Seq.head |> entityToSession |> Some

    connection.Close()
    result

let createSession (session : NewSession) = 
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
