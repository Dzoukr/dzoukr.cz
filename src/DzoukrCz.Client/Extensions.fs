[<AutoOpen>]
module DzoukrCz.Client.Extensions

open Feliz
open Router

type prop
    with
        static member inline hrefRouted (p:Page) =
            [
                prop.href (p |> Page.toUrlSegments |> Router.formatPath)
                prop.onClick Router.goToUrl
            ]

type Html
    with
        static member inline a (text:string, p:Page) =
            Html.a [
                yield! prop.hrefRouted p
                prop.text text
            ]
        static member inline classed fn (cn:string) (elm:ReactElement list) =
            fn [
                prop.className cn
                prop.children elm
            ]
        static member inline divClassed (cn:string) (elm:ReactElement list) = Html.classed Html.div cn elm
        static member inline faIcon (cn:string) = Html.i [ prop.className cn ]
        static member inline loading (cn:string) =
            Html.p [
                prop.className $"{cn} bg-slate-200 rounded"
                prop.dangerouslySetInnerHTML "&nbsp;"
            ]

module Cmd =
    open Elmish

    module OfFunc =
        /// Command to evaluate a simple function
        let func (fn: unit -> unit) : Cmd<'msg> =
            let bind dispatch =
                try fn()
                with x -> ()
            [bind]