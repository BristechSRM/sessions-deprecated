module Speakers.Repositories

open System
open Speakers.Models
open Speakers.Entities


(*
Excluded and replaced with json import until sql connection issues are fixed
open System.Configuration
open MySql.Data.MySqlClient
open Dapper


let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let connection = new MySqlConnection(connectionString)
connection.Open()

*)

open Newtonsoft.Json
open System.IO

let importFilePath = @"outlines-import.json"
let talkOutlines = JsonConvert.DeserializeObject<TalkOutlineEntity seq>(File.ReadAllText(importFilePath))

let entityToTalkOutline (entity: TalkOutlineEntity) =
    {
        TalkId = entity.TalkId;
        Title = entity.Title;
        Status = enum<TalkStatus>((int)entity.Status);
        SpeakerName = entity.SpeakerName;
        SpeakerEmail = entity.SpeakerEmail;
        SpeakerRating = enum<Rating>((int)entity.SpeakerRating);
        SpeakerLastContacted = DateTime.Now;
        AdminName = entity.AdminName;
        AdminImageUrl = entity.AdminImageUrl
    }

let getAllTalkOutlines =
    talkOutlines 
    |> Seq.map entityToTalkOutline
//    Excluded and replaced with json import until sql connection issues are fixed
//    connection.Query<TalkOutlineEntity>("select * from talk_outlines")
//    |> Seq.map entityToTalkOutline




