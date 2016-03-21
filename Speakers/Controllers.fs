namespace Speakers.Controllers

open System.Web.Http
open Speakers.Models

type TalkOutlinesController() =
    inherit ApiController()

    member __.Get() =
        printfn "Received Get Request for Talk Outlines"
        exampleTalkOutlines

type TestController() = 
    inherit ApiController()

    member __.Get() =
        "Test Return for multiple controllers!"