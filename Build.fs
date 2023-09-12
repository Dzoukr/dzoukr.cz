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
let appPublishPath = publishPath </> "app"

// Targets
let clean proj = [ proj </> "bin"; proj </> "obj"; proj </> ".fable-build" ] |> Shell.cleanDirs

Target.create "Clean" (fun _ ->
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
    run Tools.dotnet "fable clean --yes" ""
    run Tools.yarn "build" ""
)

Target.create "Run" (fun _ ->
    [
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