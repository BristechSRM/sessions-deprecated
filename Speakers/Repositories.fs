module Speakers.Repositories

open Speakers.Models
open Speakers.Entities
open System.Configuration
open MySql.Data.MySqlClient
open Dapper

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let connection = new MySqlConnection(connectionString)
connection.Open()

let entityToTalkOutline (entity: TalkOutlineEntity): TalkOutline =
    {
        TalkId = entity.TalkId;
        Title = entity.Title;
        Status = enum<TalkStatus>((int)entity.Status);
        SpeakerName = entity.SpeakerName;
        SpeakerEmail = entity.SpeakerEmail;
        SpeakerRating = enum<Rating>((int)entity.SpeakerRating);
        AdminName = entity.AdminName;
        AdminImageUrl = entity.AdminImageUrl
    }

let getAllTalkOutlines =
    connection.Query<TalkOutlineEntity>("select * from talk_outlines")
    |> Seq.map entityToTalkOutline

let getTalkOutline talkId =
    let talkOutlines = connection.Query<TalkOutlineEntity>("select * from talk_outlines where talkId = " + talkId.ToString())
    if Seq.isEmpty talkOutlines then
        None
    else 
        Some (entityToTalkOutline (Seq.head talkOutlines))



