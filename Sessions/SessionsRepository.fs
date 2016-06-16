module SessionsRepository

open Models
open Entities
open System
open System.Net
open System.Configuration
open MySql.Data.MySqlClient
open Dapper
open System.Data.SqlClient
open Serilog

//TODO Use Result DU 
//TODO common code with other repo
//TODO open Dapper.Contrib.Extensions

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
let getConnection() = new MySqlConnection(connectionString)

let sessionSummarySql = """SELECT 
        `s`.`id` AS `id`,
        `s`.`title` AS `title`,
        `s`.`description` AS `description`,
        `s`.`status` AS `status`,
        `s`.`speakerId` AS `speakerId`,
        `s`.`date` AS `date`,
        `s`.`dateAdded` AS `dateAdded`,
        `sp`.`forename` AS `speakerForename`,
        `sp`.`surname` AS `speakerSurname`,
        `sp`.`imageUrl` AS `speakerImageUrl`,
        `sp`.`rating` AS `speakerRating`,
        `sp`.`bio` AS `speakerBio`,
        `s`.`adminId` AS `adminId`,
        `a`.`forename` AS `adminForename`,
        `a`.`surname` AS `adminSurname`,
        `a`.`imageUrl` AS `adminImageUrl`
    FROM
        ((`sessions` `s`
        LEFT JOIN `profiles` `sp` ON ((`sp`.`id` = `s`.`speakerId`)))
        LEFT JOIN `profiles` `a` ON ((`a`.`id` = `s`.`adminId`)))"""

let entityToSession (entity : SessionSummaryEntity) : Session =
    let speaker =
        { Id = entity.SpeakerId
          Forename = entity.SpeakerForename
          Surname = entity.SpeakerSurname
          Rating = enum entity.SpeakerRating
          ImageUri = entity.SpeakerImageUrl
          Bio = entity.SpeakerBio }
    let admin =
        if entity.AdminId = Guid.Empty then None
        else Some { AdminSummary.Id = entity.AdminId
                    Forename = entity.AdminForename
                    Surname = entity.AdminSurname
                    ImageUri = entity.AdminImageUrl }
    { Id = entity.Id
      Title = entity.Title
      Description = entity.Description
      Status = entity.Status
      Date = Option.ofNullable entity.Date
      DateAdded = entity.DateAdded
      Speaker = speaker
      Admin = admin }

let sessionToEntity (session : NewSession) : SessionEntity =
    { Id = session.Id
      Title = session.Title
      Description = session.Description
      Status = session.Status
      Date = session.Date |> Option.toNullable
      SpeakerId = session.SpeakerId
      AdminId = session.AdminId
      DateAdded = 
        match session.DateAdded with
        | None -> DateTime.UtcNow
        | Some date -> date }

let getSessions() = 
    try
        use connection = getConnection()
        connection.Open()

        let result = connection.Query<SessionSummaryEntity>(sessionSummarySql)
        result |> Seq.map entityToSession
    with
    | ex ->
        Log.Error("getSessions() - Exception: {0}", ex)
        Seq.empty


type SessionSelectArgs = 
    { SessionId : Guid }

let getSession (id : Guid) =
    try
        use connection = getConnection()
        connection.Open()

        let args = { SessionId = id }
        let sessions = connection.Query<SessionSummaryEntity>(sessionSummarySql + " WHERE `s`.`id` = @SessionId", args)
        let result = 
            if Seq.isEmpty sessions then None
            else sessions |> Seq.head |> entityToSession |> Some

        result
    with
    | ex ->
        Log.Error("getSession(id) - Exception: {0}", ex)
        None


let createSession (session : NewSession) = 
    try
        use connection = getConnection()
        connection.Open()
        use transaction = connection.BeginTransaction()

        let args = { session with Id = Guid.NewGuid() } |> sessionToEntity

        connection.Execute("insert into sessions(id, title, status, date, speakerId, adminId, dateAdded) values 
            (@Id, @Title, @Status, @Date, @SpeakerId, @AdminId, @DateAdded)", args) |> ignore

        transaction.Commit()

        Success args.Id
    with 
    | ex ->
        Log.Error("createSession() - Exception: {0}", ex)
        Failure { HttpStatus = HttpStatusCode.BadRequest; Message = ex.Message}
