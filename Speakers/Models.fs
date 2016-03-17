namespace Speakers.Models

open System
open Speakers.Entities

type TalkOutline = {
    Title : string
    Status: TalkStatus
    SpeakerName : string
    SpeakerRating: Rating
    SpeakerLastContacted: DateTime
    AdminName: string
    AdminImageUrl: string
}

[<AutoOpen>]
module Helpers = 
    let createTalkOutline (talk : Talk) = 
        {
            SpeakerName = talk.Speaker.Name
            Title = talk.Title
            SpeakerRating = talk.Speaker.Rating
            AdminName = talk.Admin.Name
            AdminImageUrl = talk.Admin.ImageUrl
            SpeakerLastContacted = talk.Speaker.LastContacted
            Status = talk.Status
        }

    let exampleTalkOutlines = 
        exampleTalks |> Seq.map createTalkOutline

