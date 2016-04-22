namespace Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open SessionsRepository
open Serilog
open Models
 
type SessionsController() =
    inherit ApiController()

    member x.Get() =
        Log.Information("Received GET request for sessions")
        let sessions = getSessions()
        x.Request.CreateResponse(sessions)


    member x.Get(id: Guid) =
        Log.Information("Received GET request for a session with id {id}", id)
        let session = getSession id
        match session with
        | Some session -> x.Request.CreateResponse(session)
        | None -> x.Request.CreateResponse(HttpStatusCode.NotFound)

    member x.Post(newSessionDetail : Session) = 
        Log.Information("Received POST request for new session: {@SessionDetail}", newSessionDetail)
        match SessionsRepository.createSession newSessionDetail with
        | Success sessionId -> x.Request.CreateResponse(HttpStatusCode.Created, sessionId)
        | Failure error -> x.Request.CreateResponse(error.HttpStatus, error.Message)