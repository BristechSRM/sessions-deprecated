namespace Speakers.Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Speakers.Repositories


type TalkOutlinesController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received GET request for talk outlines"
        let talkOutlines = getAllTalkOutlines
        x.Request.CreateResponse(talkOutlines)
