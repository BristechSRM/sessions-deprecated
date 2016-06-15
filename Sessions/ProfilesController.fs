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
        | Success profile -> x.Request.CreateResponse(profile)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)
    
    //TODO validation
    member x.Post(newProfile : Profile) = 
        Log.Information("Recieved Post request for new profile")
        match ProfilesRepository.addProfile newProfile with
        | Success id -> x.Request.CreateResponse(HttpStatusCode.Created, id)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)

    member x.Put(id: Guid, updatedProfile : Profile) = 
        Log.Information("Recieved Put request for profile with id {id}", id)
        match ProfilesRepository.updateProfile id updatedProfile with
        | Success _ -> x.Request.CreateResponse(HttpStatusCode.OK)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)