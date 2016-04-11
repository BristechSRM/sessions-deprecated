module Sessions.Repositories

open Sessions.Models
open Sessions.Entities
open System
open System.Configuration
open MySql.Data.MySqlClient
open Dapper
open Serilog

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let connection = new MySqlConnection(connectionString)
connection.Open()

let entityToSessionDetail (entity: SessionEntity): SessionDetail =
    {
        Id = entity.Id
        Title = entity.Title;
        Status = entity.Status;
        SpeakerId = entity.SpeakerId;
        AdminId = entity.AdminId;
        ThreadId = entity.ThreadId;
    }

let entityToSessionSummary (entity: SessionEntity): SessionSummary =
    {
        Id = entity.Id;
        Title = entity.Title;
        Status = entity.Status;
        SpeakerId = entity.SpeakerId;
        AdminId = entity.AdminId;
    }

let getAllSessions () =
    connection.Query<SessionEntity>("SELECT * FROM sessions")
    |> Seq.map entityToSessionSummary

type SessionSelectArgs = { SessionId: Guid }

let getSession (id: Guid) =
    let args = { SessionId = id }
    let sessions = connection.Query<SessionEntity>("SELECT * FROM sessions WHERE id = @SessionId", args)
    if Seq.isEmpty sessions then
        None
    else
        Some (entityToSessionDetail (Seq.head sessions))

let executeInTransaction (command : MySqlCommand) = 
    use transaction = connection.BeginTransaction()
    command.ExecuteNonQuery() |> ignore
    transaction.Commit()

let createSession (sessionsDetail : SessionDetail) = 
    let commandStr = "insert into sessions(id, title, status, speakerId, adminId, threadId) values 
        (@id, @title, @status, @speakerId, @adminId, @threadId)"
    let command = new MySqlCommand(commandStr, connection)
    let id = Guid.NewGuid()
    command.Parameters.Add("@id", MySqlDbType.Guid).Value <- id
    command.Parameters.Add("@title", MySqlDbType.String).Value <- sessionsDetail.Title
    command.Parameters.Add("@status", MySqlDbType.Enum).Value <- sessionsDetail.Status
    command.Parameters.Add("@speakerId", MySqlDbType.Guid).Value <- sessionsDetail.SpeakerId
    command.Parameters.Add("@adminId", MySqlDbType.Guid).Value <- sessionsDetail.AdminId
    command.Parameters.Add("@threadId", MySqlDbType.Guid).Value <- sessionsDetail.ThreadId
    command |> executeInTransaction |> ignore

    id
