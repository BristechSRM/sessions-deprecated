namespace Sessions.Controllers

open System.Net
open System.Net.Http
open System.Web.Http
open Sessions.Repositories
open Serilog

type SessionsController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for sessions")
        let sessions = getAllSessions
        x.Request.CreateResponse(sessions)

    member x.Get(id: int) =
        Log.Information("Received GET request for session with id {id}", id)
        let session = getSession id
        match session with
        | Some session -> x.Request.CreateResponse(session)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)