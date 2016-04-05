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

let entityToSession (entity: SessionEntity): Session =
    {
        Id = new Guid(entity.Id);
        Title = entity.Title;
        Status = enum<TalkStatus>((int)entity.Status);
        SpeakerId = new Guid(entity.SpeakerId);
        AdminId = new Guid(entity.AdminId);
        ThreadId = new Guid(entity.ThreadId);
    }
    
let entityToShortFormSession (entity: SessionEntity): ShortFormSession =
    {
        Id = new Guid(entity.Id);
        Title = entity.Title;
        Status = enum<TalkStatus>((int)entity.Status);
        SpeakerId = new Guid(entity.SpeakerId);
        AdminId = new Guid(entity.AdminId);
    }

let getAllSessions () =
    connection.Query<SessionEntity>("select * from sessions")
    |> Seq.map entityToShortFormSession

let getSession id =
    let sessions = connection.Query<SessionEntity>("select * from sessions where id = " + id.ToString())
    if Seq.isEmpty sessions then
        None
    else 
        Some (entityToSession (Seq.head sessions))