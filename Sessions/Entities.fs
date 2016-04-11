namespace Sessions.Entities
open System

open System

type SessionEntity = {
    Id: Guid
    Title: string
    Status: string
    SpeakerId: Guid
    AdminId: Guid
    ThreadId: Guid
}