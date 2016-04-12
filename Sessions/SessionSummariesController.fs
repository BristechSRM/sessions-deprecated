namespace Controllers

open System.Net.Http
open System.Web.Http
open SessionsRepository
open Serilog

type SessionSummariesController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for all sessions")
        let sessions = getSessionSummaries()
        x.Request.CreateResponse(sessions)