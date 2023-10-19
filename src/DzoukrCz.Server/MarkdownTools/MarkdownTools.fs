module DzoukrCz.Server.MarkdownTools.MarkdownTools

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

    member this.GetCellValue(rowIndex:int, cellName:string) =
        let index = cellIndexes.[cellName]
        dic.[rowIndex + 1].[index]

    member this.GetRowValues (i:int) = dic.[i]

let findTables (md:string) : TableReader list =
    let doc = Markdown.Parse(md, pipeline)
    doc.Descendants<Table>()
    |> Seq.map tableToDict
    |> Seq.map TableReader
    |> Seq.toList