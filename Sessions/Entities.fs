namespace Entities

open System
open Dapper.Contrib.Extensions

type SessionEntity = 
    { Id : Guid
      Title : string
      Status : string
      SpeakerId : Guid
      AdminId : Guid
      ThreadId : Guid }

[<CLIMutable>]
type SessionSummaryEntity = 
    { Id : Guid
      Title : string
      Status : string
      SpeakerName : string
      SpeakerRating : int
      AdminName : string
      AdminImageUrl : string}

[<Table("profiles")>]
type ProfileEntity = 
    { [<Key>]
      Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUrl : string }

[<Table("handles")>]
type HandleEntity = 
    { Id : int
      Type : string
      Identifier : string
      [<Key>]
      ProfileId : Guid }

