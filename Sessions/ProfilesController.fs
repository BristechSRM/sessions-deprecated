namespace Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open Models

type ProfilesController() = 
    inherit ApiController()
    
    member x.Get(id : Guid) = 
        Log.Information("Recived GET request for profile with id {id}", id)
        match ProfilesRepository.getProfile id with
        | Some profile -> x.Request.CreateResponse(profile)
        | None -> x.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Profile not found")
    
    //TODO validation
    member x.Post(newProfile : Profile) = 
        Log.Information("Recieved Post request for new profile")
        match ProfilesRepository.addProfile newProfile with
        | Some id -> x.Request.CreateResponse(HttpStatusCode.Created, id)
        | None -> x.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Profile")
