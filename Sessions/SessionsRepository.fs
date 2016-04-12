module SessionsRepository

open Models
open Entities
open System
open System.Configuration
open MySql.Data.MySqlClient
open Dapper

//TODO Use Result DU 
//TODO use connection per query
//TODO common code with other repo

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString
let connection = new MySqlConnection(connectionString)

connection.Open()

let entityToSessionDetail (entity : SessionEntity) : SessionDetail = 
    { Id = entity.Id
      Title = entity.Title
      Status = entity.Status
      SpeakerId = entity.SpeakerId
      AdminId = entity.AdminId
      ThreadId = entity.ThreadId }

let getSessionSummaries() = connection.Query<SessionSummaryEntity>("SELECT * FROM session_summaries")

type SessionSelectArgs = 
    { SessionId : Guid }

let getSession (id : Guid) = 
    let args = { SessionId = id }
    let sessions = connection.Query<SessionEntity>("SELECT * FROM sessions WHERE id = @SessionId", args)
    if Seq.isEmpty sessions then None
    else Some(entityToSessionDetail (Seq.head sessions))

let createSession (sessionDetail : SessionDetail) = 
    let args = { sessionDetail with Id = Guid.NewGuid() }
    use transaction = connection.BeginTransaction()
    connection.Execute("insert into sessions(id, title, status, speakerId, adminId, threadId) values 
        (@Id, @Title, @Status, @SpeakerId, @AdminId, @ThreadId)", args) |> ignore
    transaction.Commit()
    args.Id
