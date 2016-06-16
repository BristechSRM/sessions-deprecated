namespace Controllers

open System
open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open Models
open ProfilesRepository

type ProfilesController() = 
    inherit ApiController()
    
    member x.Get(id : Guid) = 
        Log.Information("Received GET request for profile with id {id}", id)
        match getProfile id with
        | Success profile -> x.Request.CreateResponse(profile)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)
    
    //TODO validation
    member x.Post(newProfile : Profile) = 
        Log.Information("Received Post request for new profile")
        match addProfile newProfile with
        | Success id -> x.Request.CreateResponse(HttpStatusCode.Created, id)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)

    member x.Put(id: Guid, updatedProfile : Profile) = 
        Log.Information("Received Put request for profile with id {id}", id)
        match updateProfile id updatedProfile with
        | Success _ -> x.Request.CreateResponse(HttpStatusCode.OK)
        | Failure error -> x.Request.CreateErrorResponse(error.HttpStatus, error.Message)