namespace Speakers.Controllers

open System
open System.Web.Http
open Speakers.Models

type TalkOutlinesController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received Get Request for Talk Outlines"
        exampleTalkOutlines
