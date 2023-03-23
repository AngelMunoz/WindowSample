namespace WindowSample

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.Platform.Storage
    open Avalonia.FuncUI.Builder

    // IWritable is generally the store of a value that can be changed
    // by anyone who can call its  "Set" function
    let handleClick (window: Window, paths: IWritable<(string * string) list>) =
        async {
            let options =
                FilePickerOpenOptions(
                    AllowMultiple = true,
                    FileTypeFilter = [ FilePickerFileType("Markdown Files", Patterns = [ "*.md"; "*.markdown" ]) ]
                )

            let! files = window.StorageProvider.OpenFilePickerAsync(options) |> Async.AwaitTask

            paths.Set(files |> Seq.toList |> List.map (fun file -> file.Name, file.Path.AbsolutePath))
        }
        |> Async.Start

    let mapToNameAndPath files =
        files |> List.map (fun (name, path) -> $"- {name}: {path}")

    let view (window: Window) =

        Component(fun ctx ->
            // Create a state value, this will help funcui track changes
            let filePaths = ctx.useState List.empty<string * string>

            let nameAndPath =
                filePaths
                // writable values can be used as the source of information to perform transformations
                // once a transformation has been performed on then they will become IReadable
                // as they will change any time the source IWritable gets updated
                |> State.readMap mapToNameAndPath

            StackPanel.create
                [
                    StackPanel.orientation Orientation.Horizontal
                    StackPanel.verticalAlignment VerticalAlignment.Center
                    StackPanel.horizontalAlignment HorizontalAlignment.Center
                    StackPanel.spacing 12.
                    StackPanel.children
                        [
                            StackPanel.create
                                [
                                    StackPanel.spacing 8.
                                    StackPanel.children
                                        [
                                            TextBlock.create [ TextBlock.text "Component Model" ]
                                            Button.create
                                                [
                                                    Button.content "Open Dialog"
                                                    // async work can be performed inline or deferred to another function
                                                    Button.onClick (fun _ -> handleClick (window, filePaths))
                                                ]
                                            for file in nameAndPath.Current do
                                                TextBlock.create [ TextBlock.text file ]
                                        ]
                                ]
                            StackPanel.create
                                [
                                    StackPanel.spacing 8.
                                    StackPanel.children
                                        [
                                            TextBlock.create [ TextBlock.text "Elmish Model" ]
                                            // Include an existing elmish component into another FuncUI component
                                            ViewBuilder.Create<Elmish.Counter.ElmishComponent>([])
                                        ]
                                ]
                        ]
                ])
