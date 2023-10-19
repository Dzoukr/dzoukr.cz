module DzoukrCz.Server.Program

open Azure.Storage.Blobs
open DzoukrCz.Server.AzureBlobFileProvider
open DzoukrCz.Server.MoonServer.StoragePublisher
open Microsoft.AspNetCore.Builder
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
    builder.Services.AddSingleton<Configuration>(
        {
            ConnectionString = storageConnectionString
            TableName = tableName
            ContainerName = containerName
            PathPrefix = pathPrefix
        })
        |> ignore
    builder.Services.AddSingleton<BlobFileProvider>(BlobFileProvider(BlobContainerClient(storageConnectionString, containerName))) |> ignore
    builder.Services.AddScoped<Publisher>() |> ignore
    builder

let private configureApp (app:WebApplication) =
    let blobFileProvider = (app :> IApplicationBuilder).ApplicationServices.GetRequiredService<BlobFileProvider>()
    let pathPrefix = app.Configuration.["PathPrefix"]
    app.UseCors() |> ignore
    app.UseStaticFiles(StaticFileOptions(RequestPath = pathPrefix, FileProvider = blobFileProvider)) |> ignore
    app.UseStaticFiles() |> ignore
    app.UseGiraffe WebApp.webApp
    app

let private builderOptions = WebApplicationOptions(WebRootPath = "public")
let private builder =
    WebApplication.CreateBuilder(builderOptions)
    |> addApplicationInsights
    |> configureLogging
    |> configureWeb

let app =
    builder.Build()
    |> configureApp

app.Run()