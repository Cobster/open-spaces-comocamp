open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.EndpointRouting


let configureServices (services :IServiceCollection) = 
    services
        .AddRouting()
        .AddGiraffe() |> ignore

let configureApp (hostEnv :IWebHostEnvironment) (appBuilder :IApplicationBuilder) = 
    appBuilder
        .UseRouting()
        .UseEndpoints(fun e -> e.MapGiraffeEndpoints(OpenSpaces.Routes.endpoints)) |> ignore

[<EntryPoint>]
let main args =

    let builder = WebApplication.CreateBuilder(args)
    configureServices builder.Services

    let app = builder.Build()
    configureApp app.Environment app
    app.Run()

    0 // Exit code
