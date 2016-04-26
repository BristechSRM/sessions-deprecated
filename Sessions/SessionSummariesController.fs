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
        let sessions = getSessionSummaries()
        x.Request.CreateResponse(sessions)

    member x.Get(id: Guid) =
        Log.Information("Received GET request for a session with id {id}", id)
        let session = getSessionSummary id
        match session with
        | Some session -> x.Request.CreateResponse(session)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)