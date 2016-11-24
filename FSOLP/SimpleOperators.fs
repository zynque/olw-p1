#light

module FSOLP.SimpleOperators

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

let infixPrimitiveTypeModel f n s =
    {
        name = n;
        definitionList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsInt (render child rerender String)
                        let intChildren = List.map extractor se.contents
                        let result = List.reduce f intChildren
                        Data(IntPhrase(result))
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsInt (render child rerender WPF)
                        let intChildren = List.map extractor se.contents
                        let result = List.reduce f intChildren
                        Data(IntPhrase(result))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsInt (render child rerender Editor)
                        let intChildren = List.map extractor se.contents
                        let result = List.reduce f intChildren
                        Data(IntPhrase(result))
                }
            ]
        quoteList =
            [
                {
                    context = String;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsString (quote child rerender String)
                        let stringChildren = List.map extractor se.contents
                        coreDefaultContextFor n stringChildren
                }
                {
                    context = WPF;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsDisplayable (quote child rerender WPF)
                        let displayableChildren = List.map extractor se.contents
                        let result = interleave (Text(s)) displayableChildren
                        Data(DisplayablePhrase(Columns(result)))
                }
                {
                    context = Editor;
                    definition = fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let extractor = fun child -> phraseAsDisplayable (quote child rerender Editor)
                        let displayableChildren = List.map extractor se.contents
                        let result = interleave (Text(s)) displayableChildren
                        Data(DisplayablePhrase(Columns(result)))
                }
            ]
    }
    
let infixPrimitiveType binaryFunction name symbol =
    let model = infixPrimitiveTypeModel binaryFunction name symbol
    generatePrimitiveTypePhrase model

let addPrimitive = infixPrimitiveType (+) "Add" "+"
let subtractPrimitive = infixPrimitiveType (-) "Subtract" "-"
let multiplyPrimitive = infixPrimitiveType (*) "Multiply" "*"
let dividePrimitive = infixPrimitiveType (/) "Divide" "/"
