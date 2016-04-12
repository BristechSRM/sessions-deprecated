namespace Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open SessionsRepository
open Serilog
open Models
open System.Data.SqlClient
 
type SessionsController() =
    inherit ApiController()

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