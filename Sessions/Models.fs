namespace Sessions.Models

open System
open Newtonsoft.Json

[<CLIMutableAttribute>]
type SessionDetail = {
    Id : Guid
    Title : string
    Status: String
    [<JsonProperty(Required = Required.Always)>]
    SpeakerId : Guid
    AdminId: Guid
    ThreadId: Guid
}

type SessionSummary = {
    Id : Guid
    Title : string
    Status: String
    SpeakerId : Guid
    AdminId: Guid
}