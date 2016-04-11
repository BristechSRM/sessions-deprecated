namespace Sessions.Models

open System
open Newtonsoft.Json

[<CLIMutableAttribute>]
type SessionDetail = {
    Id : Guid
    Title : string
    [<JsonProperty(Required = Required.Always)>]
    Status: String
    [<JsonProperty(Required = Required.Always)>]
    SpeakerId : Guid
    AdminId: Guid
    ThreadId: Guid
}

type SessionSummary = 
    { Id : Guid
      Title : string
      Status : String
      SpeakerId : Guid
      AdminId : Guid }

type Rating = 
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5

type HandleType = 
    | Email
    | Mobile
    | Twitter
    | Meetup

type HandleSummary = 
    { Type : HandleType
      Id : string }

[<CLIMutable>]
type ProfileWithId = 
    { Id : Guid
      Forename : string
      Surname : string
      Rating : Rating
      Handles : HandleSummary [] }
