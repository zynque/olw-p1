#light

module FSOLP.DisplayableEntities

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render
open FSOLP.Context
open FSOLP.PhraseHelpers
open FSOLP.Deltas

//-----------------------------------------------------------------------------

let columnsPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "Columns";
            definitionList =
                [
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsString (render child rerender String)
                            let stringChildren = List.map extractor se.contents
                            let result = List.reduce (fun a b -> a + " " + b) stringChildren
                            Data(StringPhrase(result))
                    }
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable (render child rerender WPF)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Columns(displayableChildren)))
                    }
                    {
                        context = Editor;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable (render child rerender Editor)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Columns(displayableChildren)))
                    }
                ]
            quoteList =
                [
                    {
                        context = Editor;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable (quote child rerender Editor)
                            let displayableChildren = List.map extractor se.contents
                            let commaSeparatedChildren = interleave (Text ", ") displayableChildren
                            Data(DisplayablePhrase (Columns[Text "Columns("; Columns commaSeparatedChildren; Text ")"]))
                    }
                ]
        }

let rowsPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "Rows";
            definitionList = 
                [
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsString (render child rerender String)
                            let stringChildren = List.map extractor se.contents
                            let result = List.reduce (fun a b -> a + "\n" + b) stringChildren
                            Data(StringPhrase(result))
                    }
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable (render child rerender WPF)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Rows(displayableChildren)))
                    }
                    {
                        context = Editor;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable (render child rerender Editor)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Rows(displayableChildren)))
                    }
                ]
            quoteList = []
        }

let boxPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "Box";
            definitionList = 
                [
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let child = render se.contents.Head rerender String
                            Data(StringPhrase("[" + phraseAsString child + "]"))
                    }
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let child = render se.contents.Head rerender WPF
                            Data(DisplayablePhrase(Box(phraseAsDisplayable child)))
                    }
                    {
                        context = Editor;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let child = render se.contents.Head rerender Editor
                            Data(DisplayablePhrase(Box(phraseAsDisplayable child)))
                    }
                ]
            quoteList = []
        }

let arrowPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "arrow";
            definitionList = 
                [
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            Data(StringPhrase("->"))
                    }
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            Data(DisplayablePhrase(Arrow))
                    }
                    {
                        context = Editor;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            Data(DisplayablePhrase(Arrow))
                    }
                ]
            quoteList = []
        }
        
let undoButtonType =
    generatePrimitiveTypePhrase
        {
            name = "UndoButton";
            definitionList = 
                [
                    {
                        context = Editor;
                        definition = fun phrase renderContext ->
                            let action = fun () ->
                                if renderContext.undoStack.IsEmpty
                                    then
                                        ()
                                    else
                                        let delta = renderContext.undoStack.Head
                                        let remainder = renderContext.undoStack.Tail
                                        renderContext.undoStack <- remainder
                                        renderContext.redoStack <- delta :: renderContext.redoStack
                                        unapplyDelta delta |> ignore
                                        renderContext.rerender()
                            Data(DisplayablePhrase(Button("Undo", action)))
                    }
                ]
            quoteList = []
        }

let redoButtonType =
    generatePrimitiveTypePhrase
        {
            name = "RedoButton";
            definitionList = 
                [
                    {
                        context = Editor;
                        definition = fun phrase renderContext ->
                            let action = fun () ->
                                if renderContext.redoStack.IsEmpty
                                    then
                                        ()
                                    else
                                        let delta = renderContext.redoStack.Head
                                        let remainder = renderContext.redoStack.Tail
                                        renderContext.redoStack <- remainder
                                        renderContext.undoStack <- delta :: renderContext.undoStack
                                        applyDelta delta |> ignore
                                        renderContext.rerender()
                            Data(DisplayablePhrase(Button("Redo", action)))
                    }
                ]
            quoteList = []
        }
