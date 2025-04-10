[<AutoOpen>]
module DzoukrCz.WebServer.Extensions

open Feliz.ViewEngine

type Html
    with
        static member inline classed fn (cn:string) (elm:ReactElement list) =
            fn [
                prop.className cn
                prop.children elm
            ]
        static member inline divClassed (cn:string) (elm:ReactElement list) = Html.classed Html.div cn elm
        static member inline faIcon (cn:string) = Html.i [ prop.className cn ]
