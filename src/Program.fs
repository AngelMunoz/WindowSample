﻿namespace WindowSample

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Input
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "WindowSample"
        base.Width <- 400.0
        base.Height <- 400.0
        this.Content <- Counter.view (this)

//this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
//this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true


type App() =
    inherit Application()

    override this.Initialize() = this.Styles.Add(FluentTheme())

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args)
