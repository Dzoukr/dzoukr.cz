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
let toolsPath = Path.getFullName "tools"
let clientSrcPath = srcPath </> "DzoukrCz.Client"
let serverSrcPath = srcPath </> "DzoukrCz.Server"
let clientPublishPath = publishPath </> "app" </> "client"
let serverPublishPath = publishPath </> "app" </> "server"
let fableBuildPath = clientSrcPath </> ".fable-build"
let infrastructurePublishPath = publishPath </> "infrastructure"

// Targets
let clean proj = [ proj </> "bin"; proj </> "obj" ] |> Shell.cleanDirs

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    Tools.node "--version" clientSrcPath
    printfn "Yarn version:"
    Tools.yarn "--version" clientSrcPath
    Tools.yarn "install --frozen-lockfile" clientSrcPath
)

Target.create "PublishClient" (fun _ ->
    [ clientPublishPath ] |> Shell.cleanDirs
    Tools.dotnet (sprintf "fable --outDir %s --run webpack-cli -p" fableBuildPath) clientSrcPath
)

Target.create "PublishServer" (fun _ ->
    [ serverPublishPath ] |> Shell.cleanDirs
    let publishArgs = sprintf "publish -c Release -o \"%s\"" serverPublishPath
    Tools.dotnet publishArgs serverSrcPath
    [ serverPublishPath </> "appsettings.Development.json" ] |> File.deleteAll
)

Target.create "PublishInfrastructure" (fun _ ->
    Directory.ensure infrastructurePublishPath
    "Infrastructure.fsx" |> Shell.copyFile infrastructurePublishPath
)

Target.create "Run" (fun _ ->
    let server = async {
        Tools.dotnet "watch run" serverSrcPath
    }
    let client = async {
        Tools.dotnet (sprintf "fable watch --outDir %s --run webpack-dev-server" fableBuildPath) clientSrcPath
    }
    [server;client]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

Target.create "Publish" (fun _ ->
    [
        "PublishClient"
        "PublishServer"
        "PublishInfrastructure"
    ] |> List.iter (fun t -> Target.run 1 t [])
)

let dependencies = [
    "InstallClient" ==> "PublishClient"
    "InstallClient" ==> "Run"
]

[<EntryPoint>]
let main args = runOrDefault "Run" args