namespace Sessions.Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Sessions.Repositories


type SessionsController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received GET request for talk outlines"
        let talkOutlines = getAllTalkOutlines
        x.Request.CreateResponse(talkOutlines)

    member x.Get(id: int) =
        printfn "Received GET request for talk outline with talk id %d" id
        let talkOutline = getTalkOutline id
        match talkOutline with
        | Some talkOutline -> x.Request.CreateResponse(talkOutline)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)
