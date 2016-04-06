namespace Sessions.Models

open System

type SessionDetail = {
    Id : Guid
    Title : string
    Status: String
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