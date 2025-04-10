open System.IO
open System.Net.Http
open Fake
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.Core.TargetOperators

open BuildHelpers
open BuildTools

initializeContext()

let publishPath = Path.getFullName "publish"
let srcPath = Path.getFullName "src"
let clientSrcPath = srcPath </> "DzoukrCz.Client"
let moonServerSrcPath = srcPath </> "DzoukrCz.MoonServer"
let webServerSrcPath = srcPath </> "DzoukrCz.WebServer"
let webPublishPath = publishPath </> "webApp"
let moonPublishPath = publishPath </> "moonApp"

// Targets
let clean proj = [ proj </> "bin"; proj </> "obj"; proj </> ".fable-build" ] |> Shell.cleanDirs

Target.create "Clean" (fun _ ->
    moonServerSrcPath |> clean
    clientSrcPath |> clean
)

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    run Tools.node "--version" clientSrcPath
    printfn "Yarn version:"
    run Tools.yarn "--version" clientSrcPath
    run Tools.yarn "install --frozen-lockfile" clientSrcPath
)

Target.create "Publish" (fun _ ->
    [ webPublishPath ] |> Shell.cleanDirs
    let publishArgs = sprintf "publish -c Release -o \"%s\"" webPublishPath
    run Tools.dotnet publishArgs webServerSrcPath
    [ webPublishPath </> "appsettings.Development.json" ] |> File.deleteAll
    run Tools.yarn "build" ""
)

Target.create "Run" (fun _ ->
    Environment.setEnvironVar "ASPNETCORE_ENVIRONMENT" "Development"
    [
        "moonserver", Tools.dotnet "watch run" moonServerSrcPath
        "webserver", Tools.dotnet "watch run" webServerSrcPath
        "client", Tools.yarn "start" ""
    ]
    |> runParallel
)

let dependencies = [
    "InstallClient"
        ==> "Clean"
        ==> "Publish"

    "InstallClient"
        ==> "Clean"
        ==> "Run"
]

[<EntryPoint>]
let main args = runOrDefault "Run" args