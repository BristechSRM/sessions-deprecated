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
      SpeakerName : string
      SpeakerRating : int
      AdminName : string
      AdminImageUrl : string
      ThreadId: Guid }

[<Table("profiles")>]
type ProfileEntity = 
    { [<Key>]
      Id : Guid
      Forename : string
      Surname : string
      Rating : int
      ImageUrl : string }

type HandleEntity = 
    { [<Key>]
      ProfileId : Guid 
      Type : string
      Identifier : string }

