#light

module FSOLP.SimpleTypes

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

let intType =
    generatePrimitiveTypePhrase {
        name = "Int";
        definitionList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        Data(IntPhrase(value))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        Data(IntPhrase(value))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        let action newValue = 
                            let delta = Delta(phrase, se.contents, [Data(IntPhrase(newValue))])
                            rerender.undoStack <- delta :: rerender.undoStack
                            rerender.redoStack <- []
                            applyDelta delta
                            rerender.rerender() |> ignore
                        Data(DisplayablePhrase(EditableInt(value, action)))
                }
            ]
        quoteList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        Data(StringPhrase(value.ToString()))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        Data(DisplayablePhrase(Text(value.ToString())))
                }
//                {
//                    context = Editor;
//                    definition = fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let i = se.contents.Head
//                        let action newValue = 
//                            let delta = Delta(phrase, se.contents, [IntPhrase(newValue)])
//                            rerender.undoStack <- delta :: rerender.undoStack
//                            rerender.redoStack <- []
//                            applyDelta delta
//                            rerender.rerender() |> ignore
//                        match i with
//                            | IntPhrase(value) -> DisplayablePhrase(EditableInt(value, action))
//                            | _ -> failwith("Expected IntPhrase")
//                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsInt se.contents.Head
                        Data(DisplayablePhrase(Text(value.ToString())))
                }
            ]
    }

let intToEditableType =
    generatePrimitiveTypePhrase {
        name = "IntToEditable";
        definitionList =
            [
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let renderedChild = render se.contents.Head rerender Editor
                        let i = phraseAsInt renderedChild
                        Data(DisplayablePhrase(Text(i.ToString())))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let renderedChild = render se.contents.Head rerender WPF
                        let i = phraseAsInt renderedChild
                        Data(DisplayablePhrase(Text(i.ToString())))
                }
            ]
        quoteList =
            [
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let renderedChild = quote se.contents.Head rerender Editor
                        let displayableChild = phraseAsDisplayable renderedChild
                        Data(DisplayablePhrase(Columns([Text("IntAsEditable(");displayableChild;Text(")")])))
                }
            ]
    }

let stringType =
    generatePrimitiveTypePhrase {
        name = "String";
        definitionList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsString se.contents.Head
                        Data(StringPhrase(value))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsString se.contents.Head
                        Data(DisplayablePhrase(Text(value)))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsString se.contents.Head
                        let action newValue =
                            let newPhrase = Data(StringPhrase(newValue))
                            let delta = Delta(phrase, se.contents, [newPhrase])
                            rerender.undoStack <- delta :: rerender.undoStack
                            rerender.redoStack <- []
                            applyDelta delta
                            rerender.rerender() |> ignore
                            //setFocus (phraseAsDisplayable se.interpretations.[Editor])
                        Data(DisplayablePhrase(EditableString(value, action)))
                }
            ]
        quoteList =
            [
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let value = phraseAsString se.contents.Head
                        Data(DisplayablePhrase(Text("\"" + value + "\"")))
                }
            ]
    }
