namespace Speakers.Models

open System
open Speakers.Entities

type TalkOutline = {
    TalkId : TalkId
    Title : string
    Status: TalkStatus
    SpeakerName : string
    SpeakerEmail : string
    SpeakerRating: Rating
    SpeakerLastContacted: DateTime
    AdminName: string
    AdminImageUrl: string
}

[<AutoOpen>]
module Helpers = 
    let createTalkOutline (talk : Talk) = 
        {
            TalkId = talk.Id
            Title = talk.Title
            Status = talk.Status
            SpeakerName = talk.Speaker.Name
            SpeakerEmail = talk.Speaker.Email
            SpeakerRating = talk.Speaker.Rating
            SpeakerLastContacted = talk.Speaker.LastContacted
            AdminName = talk.Admin.Name
            AdminImageUrl = talk.Admin.ImageUrl    
        }

    let exampleTalkOutlines = 
        exampleTalks |> Seq.map createTalkOutline

