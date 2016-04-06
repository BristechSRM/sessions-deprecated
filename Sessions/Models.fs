namespace Sessions.Models

open System

type SessionStatus = 
    | Unassigned = 0
    | Assigned = 1
    | InProgress = 2
    | Deferred = 3
    | TopicApproved = 4
    | DateAssigned = 5

type SessionDetail = {
    Id : Guid
    Title : string
    Status: SessionStatus
    SpeakerId : Guid
    AdminId: Guid
    ThreadId: Guid
}

type SessionSummary = {
    Id : Guid
    Title : string
    Status: SessionStatus
    SpeakerId : Guid
    AdminId: Guid
}