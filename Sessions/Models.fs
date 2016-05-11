namespace Models

open System
open System.Net
open Newtonsoft.Json 

type Result<'Success,'Failure> = 
    | Success of 'Success
    | Failure of 'Failure

type ServerError = 
    { HttpStatus : HttpStatusCode
      Message : string }

type Rating = 
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5

type SpeakerSummary =
    { Id : Guid
      Forename : string
      Surname : string
      Rating : Rating
      ImageUri : string
      Bio : string }

type AdminSummary =
    { Id : Guid
      Forename : string
      Surname : string
      ImageUri : string }

type Session =
    { Id : Guid
      Title : string
      Description : string
      Status : string
      Date : DateTime option
      DateAdded : DateTime
      Speaker : SpeakerSummary
      Admin : AdminSummary option }

[<CLIMutable>]
type NewSession =
    { Id : Guid
      Title : string
      Description : string
      [<JsonProperty(Required = Required.Always)>]
      Status : string
      Date : DateTime option
      [<JsonProperty(Required = Required.Always)>]
      SpeakerId : Guid
      AdminId : Guid
      DateAdded : DateTime option }

[<CLIMutable>]
type Handle = 
    { Type : string
      Identifier : string }

[<CLIMutable>]
type Profile = 
    { Id : Guid
      Forename : string
      Surname : string
      Rating : Rating
      ImageUrl : string
      Bio : string
      Handles : Handle seq }


