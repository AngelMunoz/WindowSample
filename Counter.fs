namespace WindowSample

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout

    let view (window: Window) =

        Component(fun ctx ->
            let filePaths = ctx.useState [||]

            let handleClickAsync () =
                let dialog =
                    let filter =
                        FileDialogFilter(Name = "Markdown Files", Extensions = ResizeArray([ "md" ]))

                    OpenFileDialog(
                        AllowMultiple = true,
                        Filters = ResizeArray([ filter ]),
                        Title = "Select Markdown Files"
                    )

                async {
                    let! files = dialog.ShowAsync window |> Async.AwaitTask
                    filePaths.Set files
                }
                |> Async.Start

            let handleClickTask () =
                let dialog =
                    let filter =
                        FileDialogFilter(Name = "Markdown Files", Extensions = ResizeArray([ "md" ]))

                    OpenFileDialog(
                        AllowMultiple = true,
                        Filters = ResizeArray([ filter ]),
                        Title = "Select Markdown Files"
                    )

                task {
                    let! files = dialog.ShowAsync window
                    filePaths.Set files
                }
                |> ignore

            StackPanel.create
                [
                    StackPanel.verticalAlignment VerticalAlignment.Center
                    StackPanel.horizontalAlignment HorizontalAlignment.Center
                    StackPanel.children
                        [
                            Button.create
                                [
                                    Button.content "Open Dialog Using Async"
                                    Button.onClick (fun _ -> handleClickAsync ())
                                ]
                            Button.create
                                [
                                    Button.content "Open Dialog Using Task"
                                    Button.onClick (fun _ -> handleClickTask ())
                                ]
                            for file in filePaths.Current do
                                TextBlock.create [ TextBlock.text file ]
                        ]
                ])
