module Sessions.Repositories

open Sessions.Models
open Sessions.Entities
open System
open System.Configuration
open MySql.Data.MySqlClient
open Dapper

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
