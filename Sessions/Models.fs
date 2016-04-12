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

[<CLIMutable>]
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

type Rating = 
    | Zero = 0
    | One = 1
    | Two = 2
    | Three = 3
    | Four = 4
    | Five = 5

type HandleType = 
    | Email
    | Github
    | Mobile
    | Twitter
    | Meetup

type Handle = 
    { Type : HandleType
      Identifier : string }

[<CLIMutable>]
type Profile = 
    { Id : Guid
      Forename : string
      Surname : string
      Rating : Rating
      ImageUrl : string
      Handles : Handle [] }
