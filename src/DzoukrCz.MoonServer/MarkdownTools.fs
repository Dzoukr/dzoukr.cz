module DzoukrCz.MoonServer.MarkdownTools

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

    member this.GetHeaders() = dic.[0]

    member this.GetValues() =
        dic.Values
        |> Seq.skip 1
        |> Seq.toArray

    member this.GetKeyValues() =
        let headers = this.GetHeaders()
        this.GetValues()
        |> Array.map (fun x ->
            x |> Array.mapi (fun i y ->
                headers.[i], y
            )
        )

    member this.GetValue(rowIndex:int, column:string) =
        let headers = this.GetHeaders()
        let headIndex = Array.IndexOf(headers, column)
        this.GetValues().[rowIndex][headIndex]


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