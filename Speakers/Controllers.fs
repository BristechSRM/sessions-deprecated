namespace Speakers.Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open Speakers.Models
open Speakers.Repositories

type TalkOutlinesController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received GET request for talk outlines"
        x.Request.CreateResponse(getAllTalkOutlines)

    member x.Get(id:int) =
        printfn "Received GET request for talk outline with talk id %d" id
        let talkOutline = getTalkOutline id
        match talkOutline with
        | Some talkOutline -> x.Request.CreateResponse(talkOutline)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)

type TestController() =
    inherit ApiController()

    member __.Get() =
        "Test Return for multiple controllers!"
