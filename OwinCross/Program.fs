open Newtonsoft.Json
open Microsoft.Owin.Hosting
open Owin
open Microsoft.Owin
open System
open System.Web
open System.Net.Http
open System.Net.Http.Headers
open System.Web.Cors
open System.Web.Http
open System.Web.Http.Cors
open System.Collections.Generic
open System.Threading

module ConfigurationHelpers =
    let configureCors (config : HttpConfiguration) =
        let cors = Cors.EnableCorsAttribute("*","*","*")
        config.EnableCors(cors)
        config

    let configureRoutes (config : HttpConfiguration) =
        let routes = config.Routes
        let route = routes.MapHttpRoute("DefaultApi", "{controller}/{id}")
        route.Defaults.Add("id", RouteParameter.Optional)
        config

    let configureSerializationFormatters (config : HttpConfiguration) =
        config.Formatters.XmlFormatter.UseXmlSerializer <- true

        //The following lines are for controlling the serialisation of F# properties from Records and Unions
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver ()

        config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling <- MissingMemberHandling.Error
        config.Formatters.JsonFormatter.SerializerSettings.Error <- new System.EventHandler<Serialization.ErrorEventArgs>(fun _ errorEvent ->
            let context = System.Web.HttpContext.Current
            let error = errorEvent.ErrorContext.Error
            context.AddError(error)
            errorEvent.ErrorContext.Handled <- true)
        config.Formatters.JsonFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue("text/html"))
        config

    let registerConfiguration (config : HttpConfiguration) =
        config
        |> configureCors
        |> configureRoutes
        |> configureSerializationFormatters

open ConfigurationHelpers

type Startup() =
   member x.Configuration (appBuilder: IAppBuilder) =
       let config = registerConfiguration( new HttpConfiguration())
       appBuilder.UseWebApi(config) |> ignore

type ValuesController() =
    inherit ApiController()
    member x.Get() =
        printfn "Received Get for Values"
        seq { yield "value11"; yield "value21"; yield "value31" }
    member x.Get(id: int) =
        printfn "Received Get for Value %d" id
        sprintf "value %d" id

type Speaker = {
    Name : string
    Title : string
    Rating: int
    Admin: string
    AdminImageUrl: string
    LastContacted: string
}

type SpeakersController() =
    inherit ApiController()

    member x.Get() =
        printfn "Received Get Request for Speakers"
        seq {
            yield {Name = "Thomas Hull"; Title = "To know javascript is to love javascript"; Rating=5; Admin="David Wybourn"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="1970-01-01"; }
            yield {Name = "Jason Ebbin";   Title = "How dull is F#"; Rating=3; Admin="Jason Ebbin"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="1989-11-09" }
            yield {Name = "David Wybourn"; Title = "Concourse: Where I met myself"; Rating=5; Admin="Chris James Smith"; AdminImageUrl="https://placebear.com/50/50"; LastContacted="2015-10-21" }
        }

[<EntryPoint>]
let main _ =
    let baseAddress = "http://*:9000"
    use webapp = WebApp.Start<Startup>(baseAddress)
    printfn "Running on %s" baseAddress
    let cancelSource = new CancellationTokenSource()
    let cancelled = cancelSource.Token.WaitHandle.WaitOne()
    0
