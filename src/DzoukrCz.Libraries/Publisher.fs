namespace DzoukrCz.Libraries.Publisher

open System
open System.Text.Json
open System.Text.Json.Nodes
open System.Threading.Tasks

type Metadata = (string * JsonNode) list

module Metadata =
    let tryGet (k:string) (m:Metadata) =
        m |> List.tryFind (fun (x,_) -> String.Compare(x,k, StringComparison.InvariantCultureIgnoreCase) = 0)
        |> Option.map (fun (_,v) -> v)

    let tryGetString (k:string) (m:Metadata)  =
        tryGet k m |> Option.map _.ToString()

    let private tryToStringList (xs:JsonArray) =
        try
            xs |> Seq.map _.ToString() |> Seq.toList |> Some
        with _ -> None

    let tryGetStrings (k:string) (m:Metadata)  =
        tryGet k m
        |> Option.bind (fun x ->
            if x.GetValueKind() = JsonValueKind.Array then x.AsArray() |> tryToStringList
            else None)
        |> Option.defaultValue []

type PublishRequest = {
    Id : string option
    Name : string
    Path : string
    Content : string
    Metadata : Metadata
    Attachments : (string * string) list
}

type PublishResponse = {
    Id : string
    Name : string
    Path : string
    Content : string
    Metadata : Metadata
    CreatedAt : DateTimeOffset
}

type Publisher =
    abstract member Publish : data:PublishRequest -> Task<string>
    abstract member Unpublish : id:string -> Task<unit>
    abstract member TryDetail : id:string -> Task<PublishResponse option>
    abstract member FindByMetadataEq : partitionKey:string option * name:string * value:JsonNode -> Task<PublishResponse list>
    abstract member FindByPartition : partitionKey:string option -> Task<PublishResponse list>