#light

module FSOLP.RenderQuoteTypes

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render
open FSOLP.Context
open FSOLP.PhraseHelpers

//-----------------------------------------------------------------------------

let renderType =
    generatePrimitiveTypePhrase {
        name = "Render";
        definitionList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let phrase = se.contents.Head
                        let contextPhrase = render se.contents.Tail.Head rerender String
                        let context = phraseAsContext contextPhrase
                        render phrase rerender context
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let phrase = se.contents.Head
                        let contextPhrase = render se.contents.Tail.Head rerender WPF
                        let context = phraseAsContext contextPhrase
                        render phrase rerender context
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let phrase = se.contents.Head
                        let contextPhrase = render se.contents.Tail.Head rerender Editor
                        let context = phraseAsContext contextPhrase
                        render phrase rerender context
                }
            ]
        quoteList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let phrase = phraseAsString (quote se.contents.Head rerender String)
                        let context = phraseAsString (quote se.contents.Tail.Head rerender String)
                        Data(StringPhrase("Render [" + phrase + "] under [" + context + "]"))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let phrase = phraseAsDisplayable (quote se.contents.Head rerender Editor)
                        let context = phraseAsDisplayable (quote se.contents.Tail.Head rerender Editor)
                        Data(DisplayablePhrase(Columns([Text("Render [");phrase;Text("] under [");context;Text("]")])))
                }
            ]
    }

let quoteType =
    generatePrimitiveTypePhrase {
        name = "Quote";
        definitionList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        quote se.contents.Head rerender String
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        quote se.contents.Head rerender WPF
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        quote se.contents.Head rerender Editor
                }
            ]
        quoteList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let result = quote se.contents.Head rerender String
                        Data(StringPhrase("\"" + phraseAsString result + "\""))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let result = quote se.contents.Head rerender WPF
                        Data(DisplayablePhrase(Columns([Text("\"");(phraseAsDisplayable result);Text("\"")])))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let result = quote se.contents.Head rerender Editor
                        let displayableResult = phraseAsDisplayable result
                        Data(DisplayablePhrase(Columns([Text("\"");displayableResult;Text("\"")])))
                }
            ]
    }
