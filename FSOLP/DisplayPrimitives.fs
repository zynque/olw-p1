#light

module FSOLP.DisplayPrimitives

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Input

//-----------------------------------------------------------------------------

type DisplayablePrimitive =
    | Text of string
    | Number of int
    | Tab
    | Rows of DisplayablePrimitive list
    | Columns of DisplayablePrimitive list
    | TextBox of string * (string -> unit)
    | Button of string * (unit -> unit)
    | HorizontalRule
    | Box of DisplayablePrimitive
    | Arrow
    | ScrollPanel of DisplayablePrimitive
    | EditableString of string * (string -> unit)
    | EditableInt of int * (int -> unit)
    | EditableRows of DisplayablePrimitive list * (unit -> unit) * (int -> unit)
    | EditableRows2 of DisplayablePrimitive list * (int -> DisplayablePrimitive) * (int -> unit)
    | RerenderablePanel of ((DisplayablePrimitive -> unit) -> DisplayablePrimitive)// * (unit -> unit) * (unit -> unit)    | Highlight of DisplayablePrimitive
    | Highlight of DisplayablePrimitive
    | SaveFileButton of string * (string -> unit)
    | LoadFileButton of string * (string -> unit)
    // experiment
    | Focusable of string
    | FocusableList of DisplayablePrimitive list

let rec displayPrimitive primitive =
    match primitive with
        | Tab -> new TextBlock(Text = "   ") :> UIElement
        | Text text -> new TextBlock(Text = text) :> UIElement
        | Number n -> new TextBlock(Text = n.ToString()) :> UIElement
        | Columns cs -> 
            let displayedColumns = List.map displayPrimitive cs
            let wrapPanel = new WrapPanel()
            for displayedColumn in displayedColumns do ignore(wrapPanel.Children.Add(displayedColumn))
            KeyboardNavigation.SetTabNavigation(wrapPanel, KeyboardNavigationMode.Contained)
            wrapPanel :> UIElement
        | Rows rs ->
            let displayedRows = List.map displayPrimitive rs
            let stackPanel = new StackPanel()
            for displayedRow in displayedRows do ignore(stackPanel.Children.Add(displayedRow))
            KeyboardNavigation.SetTabNavigation(stackPanel, KeyboardNavigationMode.Contained)
            stackPanel :> UIElement
        | FocusableList rs ->
            let displayedRows = List.map displayPrimitive rs
            let stackPanel = new StackPanel()
            for displayedRow in displayedRows do ignore(stackPanel.Children.Add(displayedRow))
            KeyboardNavigation.SetTabNavigation(stackPanel, KeyboardNavigationMode.Contained)
            stackPanel :> UIElement
        | Focusable s ->
            let text = new TextBlock(Text = s, Focusable = true)
            let border = new Border(
                                BorderThickness = new Thickness(1.0),
                                BorderBrush = Media.Brushes.Transparent,
                                CornerRadius = new CornerRadius(1.0),
                                Child = text,
                                Padding = new Thickness(1.0)
                            )
            text.GotKeyboardFocus.Add(fun args -> border.BorderBrush <- Media.Brushes.Black)
            text.LostKeyboardFocus.Add(fun args -> border.BorderBrush <- Media.Brushes.Transparent)
            text.MouseUp.Add(fun args -> System.Windows.Input.Keyboard.Focus(text) |> ignore)
            border :> UIElement
        | TextBox(content, action) ->
            let textBox = new TextBox(Text = content)
            textBox.LostFocus.Add(fun args -> action((args.Source :?> TextBox).Text)) |> ignore
            textBox  :> UIElement
        | Button(label, action) ->
            let button = new Button(Content = new TextBlock(Text = label))
            button.Click.Add(fun args -> action()) |> ignore
            button :> UIElement
        | HorizontalRule ->
            new Separator(
                    BorderThickness = new Thickness(2.0),
                    BorderBrush = Media.Brushes.Black
                        ) :> UIElement
        | Box content ->
            let border = new Border(
                                BorderThickness = new Thickness(1.0),
                                BorderBrush = Media.Brushes.Black,
                                CornerRadius = new CornerRadius(4.0),
                                Child = displayPrimitive content,
                                Margin = new Thickness(2.0),
                                Padding = new Thickness(1.0),
                                Background = Media.Brushes.AntiqueWhite
                            )
            border :> UIElement
        | Arrow -> displayPrimitive (Text "=>")
        | ScrollPanel content ->
            let scrollViewer = new ScrollViewer(Content = displayPrimitive content)
            scrollViewer :> UIElement
        | EditableInt(n, action) ->
            let value = ref n
            let wrapper = new WrapPanel()
            let text = new TextBlock(Text = n.ToString(), Foreground = Media.Brushes.Blue)
            let textBox = new TextBox(Text = n.ToString())
            
            let deactivateEdit = fun (originalValue) ->
                    let newValue = System.Int32.Parse(textBox.Text)
                    text.Text <- textBox.Text
                    wrapper.Children.Remove(textBox) |> ignore
                    wrapper.Children.Add(text) |> ignore
                    if !originalValue <> newValue then
                        originalValue := newValue
                        action (System.Int32.Parse(textBox.Text))
            
            let activateEdit = fun () ->
                    wrapper.Children.Remove(text) |> ignore
                    wrapper.Children.Add(textBox) |> ignore
                    System.Windows.Input.Keyboard.Focus(textBox) |> ignore
            
            textBox.LostFocus.Add(fun args -> deactivateEdit(value))
            text.MouseUp.Add(fun args -> activateEdit())
            
            wrapper.Children.Add(text) |> ignore
            wrapper :> UIElement
        | EditableString(initialValue, action) ->
            let value = ref initialValue
            let text = new TextBlock(Text = initialValue, Foreground = Media.Brushes.Blue, Focusable = true)
            let textBox = new TextBox(Text = initialValue)
            let border = new Border(
                                BorderThickness = new Thickness(1.0),
                                BorderBrush = Media.Brushes.Transparent,
                                CornerRadius = new CornerRadius(1.0),
                                Child = text,
                                Padding = new Thickness(1.0)
                            )
            
            let deactivateEdit() =
                    let newValue = textBox.Text
                    text.Text <- newValue
                    border.Child <- text
                    if !value <> newValue then
                        value := newValue
                        action newValue
                        //let newEditableString = action newValue
                        //let editableStringBorder = newEditableString :?> Border
                        //let newTextBlock = editableStringBorder.Child
                        //Keyboard.Focus(newTextBlock) |> ignore
                    else
                        Keyboard.Focus(text) |> ignore
            
            let activateEdit() =
                    border.Child <- textBox
                    Keyboard.Focus(textBox) |> ignore
            
            textBox.PreviewKeyDown.Add(
                fun args ->
                    if args.Key = Key.Escape then
                        textBox.Text <- !value; deactivateEdit(); args.Handled <- true
                    else if args.Key = Key.Enter then
                        deactivateEdit(); args.Handled <- true
                )
                
            textBox.LostFocus.Add(fun args -> deactivateEdit(); args.Handled <- true)
                        
            text.GotKeyboardFocus.Add(fun args -> border.BorderBrush <- Media.Brushes.Black; args.Handled <- true)
            text.LostKeyboardFocus.Add(fun args -> border.BorderBrush <- Media.Brushes.Transparent; args.Handled <- true)
            
            text.PreviewMouseDown.Add(
                fun (args) ->
                    if text.IsKeyboardFocused then
                        activateEdit()
                        args.Handled <- true
                    else
                        Keyboard.Focus(text) |> ignore
                        args.Handled <- true
                )
                
            text.KeyDown.Add(
                fun args ->
                    if args.Key = Key.Enter then
                        activateEdit()
                        args.Handled <- true
                    else if args.Key = Key.Escape then
                        args.Handled <- true
                )
                
            text.TextInput.Add(fun args -> activateEdit(); textBox.Text <- args.Text; textBox.CaretIndex <- textBox.Text.Length; args.Handled <- true)
            
            border.Child <- text
            // if selected then Keyboard.Focus(text) |> ignore //????
            border :> UIElement
        | RerenderablePanel(renderCapture) -> //, undo, redo) ->
            let wrapper = new WrapPanel()
            let callbackFunc =
                fun (newContent) ->
                    wrapper.Children.Clear() |> ignore
                    wrapper.Children.Add(displayPrimitive newContent) |> ignore
            wrapper.Children.Add(displayPrimitive (renderCapture callbackFunc)) |> ignore
            //wrapper.KeyDown.Add(fun args -> if args.Key = Key.Z && ((Keyboard.Modifiers &&& ModifierKeys.Control) <> enum 0) then undo())
            //wrapper.KeyDown.Add(fun args -> if args.Key = Key.Y && ((Keyboard.Modifiers &&& ModifierKeys.Control) <> enum 0) then redo())
            wrapper :> UIElement
        | Highlight content ->
            let border = new Border(
                                Child = displayPrimitive content,
                                Background = Media.Brushes.Honeydew
                            )
            border :> UIElement
        | SaveFileButton (caption, saveFile) ->
            let button = new Button(Content = new TextBlock(Text = caption))
            let saveDialog = new Microsoft.Win32.SaveFileDialog()
            saveDialog.DefaultExt <- ".olpd"
            saveDialog.Filter <- "OLP Documents (.olpd)|*.olpd"
            let doDialog = fun () ->
                let result = saveDialog.ShowDialog()
                if result.HasValue && result.Value then
                    saveFile saveDialog.FileName
                else
                    ()
            button.Click.Add(fun args -> doDialog())
            button :> UIElement
        | LoadFileButton (caption, loadFile) ->
            let button = new Button(Content = new TextBlock(Text = caption))
            let openDialog = new Microsoft.Win32.OpenFileDialog()
            openDialog.DefaultExt <- ".olpd"
            openDialog.Filter <- "OLP Documents (.olpd)|*.olpd"
            let doDialog = fun () ->
                let result = openDialog.ShowDialog()
                if result.HasValue && result.Value then
                    loadFile openDialog.FileName
                else
                    ()
            button.Click.Add(fun args -> doDialog())
            button :> UIElement
        | EditableRows (rows, addRow, removeRow) ->
            let wrap row =
                let button = Button("-",(fun () -> ()))
                Box(Columns [row; button])
            let wrappedRows = List.append (List.map wrap rows) [Button("+",(fun () ->()))]
            displayPrimitive (Rows(wrappedRows))

        | EditableRows2 (rows, addRow, removeRow) ->
            let border = new Border()
            let stackPanel = new StackPanel()
            
            KeyboardNavigation.SetTabNavigation(stackPanel, KeyboardNavigationMode.Contained)
            
            let rec wrap row index =
                let addf () =
                    let newRow = addRow index
                    let wrappedRow = wrap newRow index
                    stackPanel.Children.Insert(index, displayPrimitive newRow)
                let add = Button("+", addf)
                let removef () =
                    removeRow index
                    stackPanel.Children.RemoveAt(index)
                let remove = Button("-", removef)
                Columns [row; add; remove]
            
            let rowsArray = List.toArray rows
            
            for i in [0 .. rowsArray.Length - 1] do
                let row = rowsArray.[i]
                let wrappedRow = displayPrimitive (wrap row i)
                stackPanel.Children.Add wrappedRow |> ignore
            
            border.Child <- stackPanel
            border :> UIElement
