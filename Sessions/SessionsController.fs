namespace Sessions.Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open Sessions.Repositories
open Serilog
open Sessions.Models
open System.Data.SqlClient
 
type SessionsController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for sessions")
        let sessions = getAllSessions()
        x.Request.CreateResponse(sessions)

    member x.Get(id: Guid) =
        Log.Information("Received GET request for session with id {id}", id)
        let session = getSession id
        match session with
        | Some session -> x.Request.CreateResponse(session)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)

    member x.Post([<FromBody>] newSessionDetail : SessionDetail) = 
        Log.Information("Received POST request for new session: {@SessionDetail}", newSessionDetail)
        try
            let sessionId = createSession newSessionDetail
            x.Request.CreateResponse(HttpStatusCode.Created, sessionId)
        with
            | :? SqlException as ex-> x.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message)
            | _ -> x.Request.CreateResponse(HttpStatusCode.InternalServerError)