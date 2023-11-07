module DzoukrCz.Client.Pages.Shares

open DzoukrCz.Client.Components
open DzoukrCz.Client.Server
open Feliz
open Elmish
open DzoukrCz.Client
open DzoukrCz.Shared.Shares.API
open Feliz.DaisyUI
open Feliz.UseElmish

type private State = {
    ShareId : string
    Share : RemoteData<Response.Share option>
}

type private Msg =
    | LoadShare
    | ShareLoaded of ServerResult<Response.Share option>

let private init (i:string) () =
    {
        ShareId = i
        Share = RemoteData.Idle
    }, Cmd.ofMsg LoadShare

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadShare -> { state with Share = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> sharesAPI.GetShare state.ShareId) ShareLoaded
    | ShareLoaded res -> { state with Share = RemoteData.setResponse res }, Cmd.none

let private inLayout (elm:ReactElement) =
    Html.divClassed "markdown-viewer prose prose-base lg:prose-xl max-w-none mx-auto w-7/12 lg:w-6/12 my-12" [
        elm
    ]

let private viewShare (share:Response.Share) =
    React.fragment [
        Html.divClassed "flex flex-col gap-2 items-end mb-8" [
            Html.divClassed "text-sm" [
                Html.faIcon "fa-regular fa-clock mr-2"
                Html.span (share.CreatedAt.ToString("dd. MM. yyyy (HH:mm)"))
            ]
        ]
        MarkdownViewer.ViewMarkdown share.Content
    ]
    |> inLayout

let private loadingDiv (wc:string) =
    Html.p [
        prop.className $"{wc} bg-slate-200 rounded"
        prop.dangerouslySetInnerHTML "&nbsp;"
    ]

let private notFoundShare =
    Daisy.alert [
        prop.children [
            Html.faIcon "fa-solid fa-circle-info"
            Html.text "This share does not exist or has already expired."
        ]
    ]
    |> inLayout

let private loadingShare =
    React.fragment [
        loadingDiv "w-6/12"
        loadingDiv "w-11/12"
        yield! [1..4] |> List.map (fun _ -> loadingDiv "w-12/12")
        loadingDiv "w-8/12"
        loadingDiv "w-7/12"
        yield! [1..4] |> List.map (fun _ -> loadingDiv "w-12/12")
    ]
    |> inLayout

[<ReactComponent>]
let SharesView (i:string) =
    let state, dispatch = React.useElmish(init i, update, [| box i |])
    match state.Share with
    // | _ -> loadingShare
    | Idle | InProgress -> loadingShare
    | Finished (Ok (Some share)) -> viewShare share
    | _ -> notFoundShare
