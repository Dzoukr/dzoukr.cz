#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.Core
open Fake.IO
open Fake.DotNet
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.Core.TargetOperators

module Tools =
    let private findTool tool winTool =
        let tool = if Environment.isUnix then tool else winTool
        match ProcessUtils.tryFindFileOnPath tool with
        | Some t -> t
        | _ ->
            let errorMsg =
                tool + " was not found in path. " +
                "Please install it and make sure it's available from your path. "
            failwith errorMsg

    let private runTool (cmd:string) args workingDir =
        let arguments = args |> String.split ' ' |> Arguments.OfArgs
        Command.RawCommand (cmd, arguments)
        |> CreateProcess.fromCommand
        |> CreateProcess.withWorkingDirectory workingDir
        |> CreateProcess.ensureExitCode
        |> Proc.run
        |> ignore

    let dotnet cmd workingDir =
        let result =
            DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
        if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

    let femto = runTool "femto"
    let node = runTool (findTool "node" "node.exe")
    let yarn = runTool (findTool "yarn" "yarn.cmd")

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

"InstallClient" ==> "PublishClient"
"InstallClient" ==> "Run"

Target.runOrDefaultWithArguments "Run"