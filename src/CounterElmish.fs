module WindowSample.Elmish.Counter

open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Platform.Storage

open Elmish

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Threading


let handleFilePicker (provider: IStorageProvider) =
    // Some async tasks need to be dispatched to the UI thread otherwise they won't update the UI
    Dispatcher.UIThread.InvokeAsync<(string * string) list>(fun _ ->
        task {
            let options =
                FilePickerOpenOptions(
                    AllowMultiple = true,
                    FileTypeFilter = [ FilePickerFileType("Markdown Files", Patterns = [ "*.md"; "*.markdown" ]) ]
                )

            let! files = provider.OpenFilePickerAsync(options)

            return files |> Seq.toList |> List.map (fun file -> file.Name, file.Path.AbsolutePath)
        })

type State =
    {
        counter: int
        files: (string * string) list
    }

type Msg =
    | Increment
    | Decrement
    | Reset
    | OpenFilePicker
    | FilePickerResponse of (string * string) list
    | Error of exn

let init () =
    {
        counter = 0
        files = List.empty<string * string>
    }

let update (storage: IStorageProvider) msg state =
    match msg with
    | Increment ->
        { state with
            counter = state.counter + 1
        },
        Cmd.none
    | Decrement ->
        { state with
            counter = state.counter - 1
        },
        Cmd.none
    | Reset -> init (), Cmd.none
    | OpenFilePicker -> state, Cmd.OfTask.either handleFilePicker storage FilePickerResponse Error
    | FilePickerResponse files -> { state with files = files }, Cmd.none
    | Error er ->
        eprintfn "%O" er
        state, Cmd.none



let mapToNameAndPath files =
    files |> List.map (fun (name, path) -> $"- {name}: {path}")

let View state dispatch =

    StackPanel.create
        [
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.children
                [
                    Button.create [ Button.content "Increment"; Button.onClick (fun _ -> dispatch Increment) ]
                    Button.create [ Button.content "Decrement"; Button.onClick (fun _ -> dispatch Decrement) ]
                    Button.create [ Button.content "Reset"; Button.onClick (fun _ -> dispatch Reset) ]

                    TextBlock.create [ TextBlock.text $"Count: {state.counter}" ]

                    Button.create
                        [
                            Button.content "Open file picker"
                            Button.onClick (fun _ -> dispatch OpenFilePicker)
                        ]

                    for file in state.files |> mapToNameAndPath do
                        TextBlock.create [ TextBlock.text file ]
                ]
        ]


type ElmishComponent() as this =
    inherit HostControl()

    let storage =
        match Avalonia.Application.Current.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as sd -> sd.MainWindow.StorageProvider
        | :? ISingleViewApplicationLifetime as sv -> TopLevel.GetTopLevel(sv.MainView).StorageProvider
        | _ -> failwith "Not Supported"

    let update = update storage

    do
        Program.mkProgram (fun _ -> init (), Cmd.none) update View
        |> Program.withHost this
        |> Program.run


//If you want to use an elmish loop as your component tree root
// you can use this window instead of the one inside Program.fs

// type ElmishWindow() as this =
//     inherit HostWindow()


//     let update = update (this.StorageProvider)

//     do
//         Program.mkProgram (fun _ -> init (), Cmd.none) update View
//         |> Program.withHost this
//         |> Program.run


// Example:


// type App() =
//     inherit Application()

//     override this.Initialize() = this.Styles.Add(FluentTheme())

//     override this.OnFrameworkInitializationCompleted() =
//         match this.ApplicationLifetime with
//         | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- ElmishWindow()
//         | _ -> ()
