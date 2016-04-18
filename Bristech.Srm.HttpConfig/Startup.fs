namespace Bristech.Srm.HttpConfig

open Owin
open System.Web.Http

type Startup() =
    member __.Configuration (appBuilder: IAppBuilder) =
        let config =
            new HttpConfiguration()
            |> Logging.configure
            |> Cors.configure
            |> Routes.configure
            |> Serialization.configure
        appBuilder.UseWebApi(config) |> ignore