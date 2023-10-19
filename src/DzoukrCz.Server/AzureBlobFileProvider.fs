module DzoukrCz.Server.AzureBlobFileProvider

open System
open System.IO
open Azure.Storage.Blobs
open Microsoft.Extensions.FileProviders

let private tryGetProperties (b:BlobClient) =
    if b.Exists().Value then b.GetProperties().Value |> Some
    else None

let private blobToFileInfo (b:BlobClient) : IFileInfo =
    let maybeProps = b |> tryGetProperties
    { new IFileInfo with
        member this.Exists = maybeProps |> Option.isSome
        member this.Length = maybeProps |> Option.map (fun x -> x.ContentLength) |> Option.defaultValue -1
        member this.PhysicalPath = null //IFileInfo.PhysicalPath docs say: Return null if the file is not directly accessible.
        member this.Name = maybeProps |> Option.map (fun _ -> b.Name) |> Option.defaultValue null
        member this.LastModified = maybeProps |> Option.map (fun x -> x.LastModified) |> Option.defaultValue DateTimeOffset.MinValue
        member this.IsDirectory = false
        member this.CreateReadStream () = maybeProps |> Option.map (fun _ -> b.OpenRead()) |> Option.defaultValue Stream.Null
    }

type BlobFileProvider (container:BlobContainerClient) =
    interface IFileProvider with
        member this.GetDirectoryContents _ = raise (NotImplementedException())
        member this.GetFileInfo(subpath) =
            let path = subpath.TrimStart('/').TrimEnd('/')
            let blob = container.GetBlobClient(path)
            blob |> blobToFileInfo
        member this.Watch _ = raise (NotImplementedException())