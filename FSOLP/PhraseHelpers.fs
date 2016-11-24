#light

module FSOLP.PhraseHelpers

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render
open FSOLP.Context
open FSOLP.MetaType

//-----------------------------------------------------------------------------

let coreDefaultContextFor name children =
    let concatinatedChildren = List.reduce (fun a b -> a + ", " + b) children
    Data(StringPhrase(name + "(" + concatinatedChildren + ")"))

let coreDefaultContextForSingle name child =
    Data(StringPhrase(name + "(" + child + ")"))

let rec interleave item list =
    match list with
        | [] -> []
        | x::[] -> [x]
        | x::xs -> x::item::(interleave item xs)

type DefinitionWithContextModel = {
        context: Context
        definition: Renderer
    }

type PhraseTypeModel = {
        name: string
        definitionList: DefinitionWithContextModel List
        quoteList: DefinitionWithContextModel List
    }

let generateDefinitionMap definitionList =
    let definitionToPair definition = (definition.context, definition.definition)
    let pairList = List.map definitionToPair definitionList
    Map.ofList pairList

let generatePrimitiveTypePhrase (model: PhraseTypeModel) =
    let phraseType = {
                        name = model.name
                        definitionMap = generateDefinitionMap model.definitionList
                        quoteMap = generateDefinitionMap model.quoteList
                     }
    Data(TypePhrase(phraseType))
