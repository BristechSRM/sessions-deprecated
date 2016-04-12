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

[<Table("profiles")>]
type ProfileEntity = 
    { [<Key>]
      Id : Guid
      Forename : string
      Surname : string
      Rating : int }
