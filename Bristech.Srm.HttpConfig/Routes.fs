namespace Bristech.Srm.HttpConfig

module Routes =
    
    open System.Web.Http
    open Serilog

    let configure (config : HttpConfiguration) =
        let routes = config.Routes
        let route = routes.MapHttpRoute("DefaultApi", "{controller}/{id}")
        route.Defaults.Add("id", RouteParameter.Optional)
        Log.Information("Configured API routing")
        config