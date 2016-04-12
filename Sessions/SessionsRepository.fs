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


let entityToSessionDetail (entity : SessionEntity) : SessionDetail = 
    { Id = entity.Id
      Title = entity.Title
      Status = entity.Status
      SpeakerId = entity.SpeakerId
      AdminId = entity.AdminId
      ThreadId = entity.ThreadId }

let getSessionSummaries() = 
    use connection = getConnection()
    connection.Open()

    let result = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries")

    connection.Close()
    result

type SessionSelectArgs = 
    { SessionId : Guid }

let getSession (id : Guid) = 
    use connection = getConnection()
    connection.Open()

    let args = { SessionId = id }
    let sessions = connection.Query<SessionEntity>("SELECT * FROM sessions WHERE id = @SessionId", args)
    let result = 
        if Seq.isEmpty sessions then None
        else Some(entityToSessionDetail (Seq.head sessions))

    connection.Close()
    result

let createSession (sessionDetail : SessionDetail) = 
    use connection = getConnection()
    connection.Open()
    use transaction = connection.BeginTransaction()

    try

        let args = { sessionDetail with Id = Guid.NewGuid() }

        connection.Execute("insert into sessions(id, title, status, speakerId, adminId, threadId) values 
            (@Id, @Title, @Status, @SpeakerId, @AdminId, @ThreadId)", args) |> ignore

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
