module HandlesController

open System
open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open Models

type HandlesController() = 
    inherit ApiController()

    
    member this.Get() =
        Log.Information("Received GET request for ALL handles")
        match ProfilesRepository.getHandles() with
        | Success handles -> this.Request.CreateResponse(handles)
        | Failure error -> this.Request.CreateErrorResponse(error.HttpStatus, error.Message)


    member this.Get(htype : string, identifier : string) = 
        Log.Information("Received GET request for handle with htype: {htype}, identifier: {identifier}", htype, identifier)
        match ProfilesRepository.getHandle htype identifier with
        | Success handle -> this.Request.CreateResponse(handle)
        | Failure error -> this.Request.CreateErrorResponse(error.HttpStatus, error.Message)
