namespace Startup

open Newtonsoft.Json
open Owin
open System.Net.Http.Headers
open System.Web.Http
open Serilog

module private ConfigurationHelpers =

    let registerConfiguration (config : HttpConfiguration) =
        config
        |> Bristech.Srm.HttpConfig.Logging.configure
        |> Bristech.Srm.HttpConfig.Cors.configure
        |> Bristech.Srm.HttpConfig.Routes.configure
        |> Bristech.Srm.HttpConfig.Serialization.configure

type Startup() =
    member __.Configuration (appBuilder: IAppBuilder) =
        let config = ConfigurationHelpers.registerConfiguration(new HttpConfiguration())        
        appBuilder.UseWebApi(config) |> ignore