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

let entityToTalkOutline (talkEntity: Sdp.dataContext.``talks.talksEntity``, speakerEntity: Sdp.dataContext.``talks.speakersEntity``, adminEntity: Sdp.dataContext.``talks.adminsEntity``) =
    {
        TalkId = (int)talkEntity.Id;
        Title = talkEntity.Title;
        Status = enum<TalkStatus>((int)talkEntity.Status);
        SpeakerName = speakerEntity.Name;
        SpeakerEmail = speakerEntity.Email;
        SpeakerRating = enum<Rating>((int)speakerEntity.Rating);
        SpeakerLastContacted = DateTime.Now;
        AdminName = adminEntity.Name;
        AdminImageUrl = adminEntity.ImageUrl;
    }

let getAllTalkOutlines =
    query {
        for talk in ctx.Talks.Talks do
        join speaker in ctx.Talks.Speakers on
            (talk.SpeakerId = speaker.Id)
        join admin in ctx.Talks.Admins on
            (talk.AdminId = admin.Id)
        select (talk, speaker, admin)
    } |> Seq.map entityToTalkOutline

let getTalkOutline index =
    let talkOutlines = query {
        for talk in ctx.Talks.Talks do
        join speaker in ctx.Talks.Speakers on
            (talk.SpeakerId = speaker.Id)
        join admin in ctx.Talks.Admins on
            (talk.AdminId = admin.Id)
        where (talk.Id = (uint32)index)
        select (talk, speaker, admin)
    }
    if Seq.isEmpty talkOutlines then
        None
    else
        let talkOutline = entityToTalkOutline (Seq.head talkOutlines)
        Some talkOutline
 