namespace Entities

open System
open Dapper.Contrib.Extensions

[<CLIMutable>]
type SessionEntity = 
    { Id : Guid
      Title : string
      Status : string
      SpeakerId : Guid
      AdminId : Guid
      ThreadId : Guid
      DateAdded : DateTime
      Date : Nullable<DateTime> }

[<CLIMutable>]
type SessionSummaryEntity = 
    { Id : Guid
      Title : string
      Status : string
      Date : Nullable<DateTime>
      SpeakerId : Guid
      SpeakerForename : string
      SpeakerSurname : string
      SpeakerImageUrl : string
      SpeakerRating : int
      AdminId : Guid
      AdminForename : string
      AdminSurname : string
      AdminImageUrl : string
      ThreadId: Guid }

[<Table("profiles")>]
type ProfileEntity = 
    { Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUrl : string }

[<CLIMutable>]
type HandleEntity = 
    { ProfileId : Guid 
      Type : string
      Identifier : string }

