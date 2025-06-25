module DzoukrCz.WebServer.Program

open Azure.Storage.Blobs
open DzoukrCz.Libraries.Publisher
open DzoukrCz.Libraries.StoragePublisher
open DzoukrCz.WebServer.Pages.Talks.Domain
open DzoukrCz.WebServer.Pages.Talks.Queries
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let private configureLogging (builder: WebApplicationBuilder) =
    builder.Host.ConfigureLogging(fun x ->
        x.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider> ("", LogLevel.Information) |> ignore
    ) |> ignore
    builder

let private addApplicationInsights (builder:WebApplicationBuilder) =
    builder.Services.AddApplicationInsightsTelemetry() |> ignore
    builder

let private configureWeb (builder:WebApplicationBuilder) =
    let storageConnectionString = builder.Configuration.["StorageAccount"]
    let tableName = builder.Configuration.["TableName"]
    let containerName = builder.Configuration.["ContainerName"]
    let pathPrefix = builder.Configuration.["PathPrefix"]

    builder.Services.AddGiraffe() |> ignore
    builder.Services.AddSingleton<StoragePublisherConfiguration>(
        {
            ConnectionString = storageConnectionString
            TableName = tableName
            ContainerName = containerName
            PathPrefix = pathPrefix
        })
        |> ignore
    builder.Services.AddSingleton<TalksQueries, StorageTalksQueries>() |> ignore
    builder.Services.AddSingleton<Publisher, StoragePublisher>() |> ignore
    builder.Services.AddMemoryCache() |> ignore
    builder.WebHost.UseUrls("http://localhost:5000") |> ignore
    builder

let private configureApp (app:WebApplication) =
    app.UseStaticFiles() |> ignore
    app.UseGiraffe WebApp.webApp
    app

let private webRootPath =
#if DEBUG
    "../DzoukrCz.WebClient/public"
#else
    "public"
#endif

let private builderOptions = WebApplicationOptions(WebRootPath = webRootPath)
let private builder =
    WebApplication.CreateBuilder(builderOptions)
    |> addApplicationInsights
    |> configureLogging
    |> configureWeb

let app =
    builder.Build()
    |> configureApp

#if DEBUG
open System.Net.Http
async {
    use client = new HttpClient()
    try
        let! _ = client.GetAsync("http://localhost:8080/__reload") |> Async.AwaitTask
        printfn "Triggered Vite page reload"
    with ex ->
        printfn "Failed to trigger reload: %s" ex.Message
} |> Async.Start
#endif

app.Run()