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

let convertToISO8601 (datetime : DateTime) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let entityToSession (entity : SessionEntity) : Session =
    { Id = entity.Id
      Title = entity.Title
      Status = entity.Status
      Date =
        if entity.Date.HasValue then
            convertToISO8601 entity.Date.Value
        else
            ""
      SpeakerId = entity.SpeakerId
      AdminId = entity.AdminId
      ThreadId = entity.ThreadId
      DateAdded = Some entity.DateAdded }

let convertToDateTime iso =
    DateTime.Parse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind)


let sessionToEntity (session : Session) : SessionEntity =
    { Id = session.Id
      Title = session.Title
      Status = session.Status
      Date =
        match session.Date with
        | null -> Nullable()
        | "" -> Nullable()
        | _ -> Nullable(convertToDateTime session.Date)

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

    let result = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries")

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
    let sessions = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries WHERE id = @SessionId", args)
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
