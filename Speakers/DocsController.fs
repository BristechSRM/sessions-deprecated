module DocsController

open Newtonsoft.Json.Linq
open System.IO
open System.Net
open System.Net.Http
open System.Web.Http
open Speakers.Repositories
open FSharp.Data

type DocsController() as this =
    inherit ApiController()

    member x.Get() =
        use stream = this.GetType().Assembly.GetManifestResourceStream("api.json")
        use streamReader = new StreamReader(stream)
        let content = streamReader.ReadToEnd()
        let docs = JObject.Parse(content);        
        x.Request.CreateResponse(HttpStatusCode.OK, docs, "application/json");