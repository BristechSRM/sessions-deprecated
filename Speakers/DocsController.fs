module DocsController

open Newtonsoft.Json.Linq
open System.IO
open System.Net
open System.Net.Http
open System.Web.Http
open Speakers.Repositories

type DocsController() =
    inherit ApiController()

    member x.Get() =                        
        let docs = JObject.Parse(File.ReadAllText("./api.json"));        
        x.Request.CreateResponse(HttpStatusCode.OK, docs, "application/json");