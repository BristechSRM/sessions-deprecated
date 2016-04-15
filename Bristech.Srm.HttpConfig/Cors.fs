namespace Bristech.Srm.HttpConfig

module Cors =

    open System.Web.Http
    open Serilog

    let configure (config : HttpConfiguration) =
        let cors = Cors.EnableCorsAttribute("*","*","*")
        config.EnableCors(cors)
        Log.Information("Configured CORS to allow all ingress")
        config