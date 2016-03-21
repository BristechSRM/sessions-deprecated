module Speakers.Repositories

open System
open FSharp.Data.Sql
open Speakers.Models
open System.Configuration

type Sdp = SqlDataProvider<
                ConnectionStringName="DefaultConnection",
                DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
                Owner = "talks"
            >

let connectionString = ConfigurationManager.ConnectionStrings.Item("DefaultConnection").ConnectionString

let ctx = Sdp.GetDataContext(connectionString)

let entityToSpeaker (entity: Sdp.dataContext.``talks.speakers_oldEntity``) =
    {
        Name = entity.Name;
        Title = entity.Title;
        Rating = enum<Rating>((int)entity.Rating);
        Admin = entity.Admin;
        AdminImageUrl = entity.AdminImageUrl;
        LastContacted = DateTime.Now;
        SpeakerStatus = enum<SpeakerStatus>((int)entity.SpeakerStatus)
    }

let getAllSpeakers =
    query {
        for s in ctx.Talks.SpeakersOld do
        select s
    } |> Seq.map entityToSpeaker

let getSpeaker index =
    let speakers = query {
        for s in ctx.Talks.SpeakersOld do
        where (s.Id = (uint32)index)
        select s
    }
    if Seq.isEmpty speakers then
        None
    else
        let speaker = entityToSpeaker (Seq.head speakers)
        Some speaker
  
let createSpeaker (speaker: Speaker) =
    let newSpeaker = ctx.Talks.SpeakersOld.Create()
    newSpeaker.Name <- speaker.Name
    newSpeaker.Title <- speaker.Title
    newSpeaker.Rating <- (uint32) speaker.Rating
    newSpeaker.Admin <- speaker.Admin
    newSpeaker.AdminImageUrl <- speaker.AdminImageUrl
    newSpeaker.SpeakerStatus <- (uint32) speaker.SpeakerStatus
    ctx.SubmitUpdates()
    newSpeaker