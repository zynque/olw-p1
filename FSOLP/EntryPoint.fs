#light

module FSOLP.EntryPoint

open System
open System.Windows
open System.Windows.Controls
open FSOLP.ApplicationContent

//-----------------------------------------------------------------------------

type MyApp() =
    inherit System.Windows.Application()
    override app.OnStartup(e) =
        base.OnStartup(e)
        let window = new Window
                        (
                            Title = "FSOLP",
                            WindowState = WindowState.Maximized,
                            Background = Media.Brushes.DarkGoldenrod
                        )
        let contentPane =  new Border(
                                BorderThickness = new Thickness(1.0),
                                BorderBrush = Media.Brushes.Black,
                                CornerRadius = new CornerRadius(8.0),
                                Margin = new Thickness(4.0),
                                Padding = new Thickness(1.0),
                                Background = Media.Brushes.AntiqueWhite
                            )
        window.Content <- contentPane
        contentPane.Child <- applicationContent()
        //window.Icon
        window.Show()

[<STAThread>]
[<EntryPoint>]
let main(args) = let app = new MyApp() in app.Run()
