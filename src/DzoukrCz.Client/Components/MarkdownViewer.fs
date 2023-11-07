module DzoukrCz.Client.Components.MarkdownViewer

open Fable.Core
open Feliz
open Fable.React

[<Import("default","remark-gfm")>]
let remarkGfm : obj = jsNative

[<ReactComponent>]
let ViewMarkdown (md : string) =
    ofImport
        "ReactMarkdownWithCode"
        "../Components/MarkdownViewerWithCode.tsx"
        {| children = md; remarkPlugins = [| remarkGfm |] |} []