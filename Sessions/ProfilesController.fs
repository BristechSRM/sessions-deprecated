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
        Log.Information("Received GET request for profile with id: {id}", id)
        match getProfile id with
        | Success profile -> 
            Log.Information("Success: Post request for profile with id: {id} complete", id)
            x.Request.CreateResponse(profile)
        | Failure error -> 
            Log.Error("Put request for profile with id: {id} failed with status code: {code} and message: {message}", id, error.HttpStatus, error.Message)
            x.Request.CreateErrorResponse(error.HttpStatus, error.Message)
    
    //TODO validation
    member x.Post(newProfile : Profile) = 
        Log.Information("Received Post request for new profile")
        match addProfile newProfile with
        | Success id -> 
            Log.Information("Success: Post request for profile with id: {id} complete", id)
            x.Request.CreateResponse(HttpStatusCode.Created, id)
        | Failure error -> 
            Log.Error("Post request for profile failed with status code: {code} and message: {message}", error.HttpStatus, error.Message)
            x.Request.CreateErrorResponse(error.HttpStatus, error.Message)

    member x.Put(id: Guid, updatedProfile : Profile) = 
        Log.Information("Received Put request for profile with id: {id}", id)
        match updateProfile id updatedProfile with
        | Success () -> 
            Log.Information("Success: Put request for profile with id: {id} complete", id)
            x.Request.CreateResponse(HttpStatusCode.OK)
        | Failure error -> 
            Log.Error("Put request for profile with id: {id} failed with status code: {code} and message: {message}", id, error.HttpStatus, error.Message)
            x.Request.CreateErrorResponse(error.HttpStatus, error.Message)