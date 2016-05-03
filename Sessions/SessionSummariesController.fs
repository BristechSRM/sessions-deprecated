namespace Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open SessionsRepository
open Serilog
open System

type SessionSummariesController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for all sessions")
        try 
            let sessions = getSessionSummaries()
            x.Request.CreateResponse(sessions)
        with
            | ex -> x.Request.CreateResponse(ex.Message)

    member x.Get(id: Guid) =
        Log.Information("Received GET request for a session with id {id}", id)
        let session = getSessionSummary id
        match session with
        | Some session -> x.Request.CreateResponse(session)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)