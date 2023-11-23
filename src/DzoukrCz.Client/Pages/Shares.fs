module DzoukrCz.Client.Pages.Shares

open System
open DzoukrCz.Client.Components
open DzoukrCz.Client.Server
open DzoukrCz.Shared.Shares.API.Response
open Feliz
open Elmish
open DzoukrCz.Client
open DzoukrCz.Shared.Shares.API
open Feliz.DaisyUI
open Feliz.UseElmish

type private State = {
    ShareId : string
    Share : RemoteData<Response.ShareWrapper option>
    UnlockShare : Request.UnlockShare
}

type private Msg =
    | LoadShare
    | ShareLoaded of ServerResult<Response.ShareWrapper option>
    | PasswordChanged of string
    | Unlock

let private init (i:string) () =
    {
        ShareId = i
        Share = RemoteData.Idle
        UnlockShare = { Id = i; Password = "" }
    }, Cmd.ofMsg LoadShare

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadShare -> { state with Share = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> sharesAPI.GetShare state.ShareId) ShareLoaded
    | ShareLoaded res -> { state with Share = RemoteData.setResponse res }, Cmd.none
    | PasswordChanged pwd -> { state with UnlockShare = { state.UnlockShare with Password = pwd } }, Cmd.none
    | Unlock -> { state with Share = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> sharesAPI.Unlock state.UnlockShare) ShareLoaded

let private inLayout (elm:ReactElement) =
    Html.divClassed "markdown-viewer prose prose-lg lg:prose-xl max-w-4xl mx-auto py-5" [
        elm
    ]

let private viewShare (isPublic:bool) (share:Response.Share) =
    let expiresIn = share.ExpiresAt |> Option.map (fun exp -> exp - DateTimeOffset.Now)
    let status =
        if isPublic then
            Html.divClassed "text-sm" [
                Html.faIcon "fa-solid fa-earth-europe mr-2"
                Html.span "Public"
            ]
        else
            Html.divClassed "text-sm" [
                Html.faIcon "fa-solid fa-unlock mr-2"
                Html.span "Unlocked"
            ]

    React.fragment [
        Html.divClassed "flex flex-col gap-2 items-end mb-8" [
            status
            Html.divClassed "text-sm" [
                Html.faIcon "fa-regular fa-clock mr-2"
                Html.span (share.CreatedAt.ToString("dd. MM. yyyy (HH:mm)"))
            ]
            match expiresIn with
            | Some exp ->
                Html.divClassed "text-sm badge badge-warning" [
                    Html.span $"Expires in {exp.TotalMinutes |> int} minutes"
                ]
            | None -> Html.none
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

let private lockedShare (state:State) dispatch =
    Daisy.modal.dialog [
        modal.open'
        prop.children [
            Daisy.modalBox.form [
                prop.onSubmit (fun x -> x.preventDefault(); dispatch Unlock)
                prop.children [
                    Daisy.alert [
                        Html.faIcon "fa-solid fa-lock"
                        Html.text "This share is locked. Please provide a password to unlock it."
                    ]
                    Daisy.input [
                        input.bordered
                        prop.className "mt-4 w-full"
                        prop.placeholder "Your password"
                        prop.onTextChange (fun pwd -> dispatch (PasswordChanged pwd))
                    ]
                    Daisy.modalAction [
                        Daisy.button.button [
                            button.primary
                            prop.type'.submit
                            prop.children [
                                Html.faIcon "fa-solid fa-unlock"
                                Html.text "Unlock"
                            ]
                            prop.disabled (state.UnlockShare.Password = String.Empty)
                        ]
                    ]
                ]
            ]
        ]
    ]

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
    | Finished (Ok (Some wrapper)) ->
        match wrapper with
        | Public share -> share |> viewShare true
        | Unlocked share -> share |> viewShare false
        | Locked -> lockedShare state dispatch
    | _ -> notFoundShare
