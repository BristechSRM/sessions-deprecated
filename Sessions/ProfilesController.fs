module ProfilesController

open System
open System.Web
open System.Net
open System.Net.Http
open System.Web.Http
open Serilog
open System.Collections.Generic

open Sessions.Models

let repoDict = new Dictionary<Guid,ProfileWithId>()
let oneTrueProfile = 
    { Id = new Guid("536c0246-6dbf-4d92-af3d-03a91b8f68ba")
      Forename = "First" 
      Surname = "Profile" 
      Rating = Rating.Five
      Handles = [| {Type = Twitter; Id ="@TheFirstProfile"} |] }

repoDict.Add(oneTrueProfile.Id,oneTrueProfile)

type ProfilesController() = 
    inherit ApiController()    

    member x.Get(id: Guid) = 
        Log.Information("Recived GET request for profile with id {id}",id)
        match repoDict.TryGetValue(id) with
        | true,profile -> x.Request.CreateResponse(profile)
        | false,_ -> x.Request.CreateErrorResponse(HttpStatusCode.NotFound,"Profile not found")

    //TODO validation
    member x.Post(newProfile : ProfileWithId) =
        Log.Information("Recieved Post request for new profile with")
        let id = Guid.NewGuid()
        let newProfile = {newProfile with Id = id}
        repoDict.Add(id,newProfile)
        Log.Information(sprintf "Repo size: %i " repoDict.Count)
        x.Request.CreateResponse(HttpStatusCode.Created,id)
