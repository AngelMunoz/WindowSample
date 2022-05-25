namespace WindowSample

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open System.Collections.Generic

    let view (window: Window) =

        Component(fun ctx ->
            let filePaths = ctx.useState []

            let handleClickAsync () =
                let dialog = OpenFileDialog()
                dialog.AllowMultiple <- true

                let filter =
                    FileDialogFilter(Extensions = List<string>([ "md" ]), Name = "Markdown Files")

                dialog.Filters.Add(filter)
                dialog.Title <- "Select Markdown Files"

                async {
                    let! files = dialog.ShowAsync window |> Async.AwaitTask
                    files |> Seq.toList |> filePaths.Set
                }

            let handleClickTask () =
                let dialog = OpenFileDialog()
                dialog.AllowMultiple <- true

                let filter =
                    FileDialogFilter(Extensions = List<string>([ "md" ]), Name = "Markdown Files")

                dialog.Filters.Add(filter)
                dialog.Title <- "Select Markdown Files"

                task {
                    let! files = dialog.ShowAsync window
                    files |> Seq.toList |> filePaths.Set
                }

            StackPanel.create
                [
                    StackPanel.verticalAlignment VerticalAlignment.Center
                    StackPanel.horizontalAlignment HorizontalAlignment.Center
                    StackPanel.children
                        [
                            Button.create
                                [
                                    Button.content "Open Dialog Using Async"
                                    Button.onClick (fun _ -> handleClickAsync () |> Async.Start)
                                ]
                            Button.create
                                [
                                    Button.content "Open Dialog Using Task"
                                    Button.onClick (fun _ -> handleClickTask () |> ignore)
                                ]
                            for file in filePaths.Current do
                                TextBlock.create [ TextBlock.text file ]
                        ]
                ])
