module DzoukrCz.Server.MarkdownTools.MarkdownTools

open System
open System.Collections.Generic
open System.IO
open Markdig
open Markdig.Extensions.Tables
open Markdig.Renderers.Normalize
open Markdig.Syntax
open Markdig.Syntax.Inlines

let private pipeline =
    MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseGridTables()
        .UsePipeTables()
        .Build()

let private toString (doc:Block) =
    let writer = new StringWriter()
    let renderer = NormalizeRenderer(writer)
    writer.Flush()
    renderer.Render(doc) |> string

let private tableToDict (doc:Table) =
    let dic = new Dictionary<int, string []>()
    doc.Descendants<TableRow>()
    |> Seq.iteri (fun i x ->
        x.Descendants<TableCell>()
        |> Seq.map toString
        |> (fun cells ->
            dic.Add(i, cells |> Seq.toArray)
        )
    )
    dic

type TableReader(dic:Dictionary<int, string []>) =
    let cellIndexes =
        dic.[0]
        |> Array.mapi (fun i x -> x, i)
        |> dict


    member this.TryGetCellValue(rowIndex:int, cellName:string) =
        match cellIndexes.ContainsKey(cellName) with
        | false -> None
        | true ->
            let index = cellIndexes.[cellName]
            Some (dic.[rowIndex + 1].[index])

    member this.GetCellValueOrEmpty(rowIndex:int, cellName:string) =
        match this.TryGetCellValue(rowIndex, cellName) with
        | None -> String.Empty
        | Some x -> x

    member this.TryGetDataRows () =
        [0 .. dic.Count - 2]
        |> List.map (fun i ->
            fun name -> this.TryGetCellValue(i, name)
        )

    member this.GetDataRowsOrEmpty () =
        [0 .. dic.Count - 2]
        |> List.map (fun i ->
            fun name -> this.GetCellValueOrEmpty(i, name)
        )

let findTables (md:string) : TableReader list =
    let doc = Markdown.Parse(md, pipeline)
    doc.Descendants<Table>()
    |> Seq.map tableToDict
    |> Seq.map TableReader
    |> Seq.toList

let trySrcAndAlt (md:string) =
    let doc = Markdown.Parse(md, pipeline)
    doc.Descendants<LinkInline>()
    |> Seq.tryHead
    |> Option.map (fun x ->
        x.Url, x.Title
    )