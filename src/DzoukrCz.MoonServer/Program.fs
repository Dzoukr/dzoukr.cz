module DzoukrCz.MoonServer.Program

open Azure.Storage.Blobs
open DzoukrCz.Libraries.StoragePublisher
open DzoukrCz.MoonServer.Security
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.EndpointRouting
open DzoukrCz.Libraries.Publisher

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
    let containerName = "$web"
    let pathPrefix = builder.Configuration.["PathPrefix"]
    let apiKey = builder.Configuration.["ApiKey"]
    let apiSecret = builder.Configuration.["ApiSecret"]

    // because of MoonServer
    builder.Services.AddCors(fun opts ->
        opts.AddDefaultPolicy (fun policy ->
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
            |> ignore
        )
    ) |> ignore
    builder.Services.AddGiraffe() |> ignore
    builder.Services.AddSingleton<StoragePublisherConfiguration>(
        {
            ConnectionString = storageConnectionString
            TableName = tableName
            ContainerName = containerName
            PathPrefix = pathPrefix
        })
        |> ignore
    builder.Services.AddSingleton<ApiSecurity>(
        {
            ApiKey = apiKey
            ApiSecret = apiSecret
        })
        |> ignore
    builder.Services.AddScoped<Publisher, StoragePublisher>() |> ignore
    builder.Services.AddMemoryCache() |> ignore
    
    builder

let private configureApp (app:WebApplication) =
    app.UseCors() |> ignore
    app.UseRouting() |> ignore
    app.UseEndpoints(fun e -> e.MapGiraffeEndpoints(WebApp.webApp)) |> ignore
    app

let private builderOptions = WebApplicationOptions()
let private builder =
    WebApplication.CreateBuilder(builderOptions)
    |> addApplicationInsights
    |> configureLogging
    |> configureWeb

let app =
    builder.Build()
    |> configureApp

app.Run()