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
        Id = new Guid(entity.Id);
        Title = entity.Title;
        Status = entity.Status;
        SpeakerId = new Guid(entity.SpeakerId);
        AdminId = new Guid(entity.AdminId);
        ThreadId = new Guid(entity.ThreadId);
    }

let entityToSessionSummary (entity: SessionEntity): SessionSummary =
    {
        Id = new Guid(entity.Id);
        Title = entity.Title;
        Status = entity.Status;
        SpeakerId = new Guid(entity.SpeakerId);
        AdminId = new Guid(entity.AdminId);
    }

let getAllSessions () =
    connection.Query<SessionEntity>("select * from sessions")
    |> Seq.map entityToSessionSummary

type SessionSelectArgs = { SessionId : string }

let getSession (id  :Guid) =
    let args = {SessionId=id.ToString()}
    let sessions = connection.Query<SessionEntity>( "select * from sessions where id = @SessionId" , args)
    if Seq.isEmpty sessions then
        None
    else
        Some (entityToSessionDetail (Seq.head sessions))
