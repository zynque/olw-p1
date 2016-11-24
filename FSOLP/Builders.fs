#light

module FSOLP.PhraseBuilders

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.ConstructDefinitionEntities
open FSOLP.PhraseExtractors
open FSOLP.Render
open FSOLP.Context
open FSOLP.SimpleTypes
open FSOLP.SimpleOperators
open FSOLP.DisplayableEntities
open FSOLP.RenderQuoteTypes
open FSOLP.LoadButtonType

//-----------------------------------------------------------------------------

let stringContext = Data(Context(String))
let editorContext = Data(Context(Editor))
let wpfContext = Data(Context(WPF))

let rerenderCapture (phrase: Phrase): ((DisplayablePrimitive -> unit) -> DisplayablePrimitive) =
    fun (updateFunc) ->
        let rec doRender = fun () -> render phrase renderContext Editor
        and renderContext =
            {
                root = phrase
                rerender = fun () -> updateFunc (phraseAsDisplayable (doRender()))
                undoStack = []
                redoStack = []
            }
        phraseAsDisplayable (doRender())
        
let mutable counter = 0
let Phrase (eType, contents) =
    let newPhrase = StandardPhrase (
                     {
                        index = counter
                        phraseType = eType
                        parents = []
                        contents = contents
                        interpretations = Map.empty<Context,Phrase>
                        quotations = Map.empty<Context,Phrase>
                     }
        )
    counter <- counter + 1
    let addParent = fun phrase ->
        match phrase with
            | StandardPhrase(sp) -> sp.parents <- List.append sp.parents [newPhrase]
            | _ -> ()
    List.iter addParent contents
    newPhrase

let showContexts expression rerender =
        render expression rerender String,
        render expression rerender WPF

let displayContexts expression rerender =
    match (showContexts expression rerender) with
        a, c -> Rows([Text(phraseAsString(a));phraseAsDisplayable(c)])

let buildQuote item = Phrase(quoteType, [item])
let buildAdd items = Phrase(addPrimitive, items)
let buildSubtract items = Phrase(subtractPrimitive, items)
let buildMultiply items = Phrase(multiplyPrimitive, items)
let buildInt i = Phrase(intType, [Data(IntPhrase i)])
let buildIntToEditable i = Phrase(intToEditableType, [i])
let buildString s = Phrase(stringType, [Data(StringPhrase s)])
let buildArrow = Phrase(arrowPrimitive, [])
let buildBox item = Phrase(boxPrimitive, [item])
let buildColumns items = Phrase(columnsPrimitive, items)
let buildRows items = Phrase(rowsPrimitive, items)
let buildSaveButton() = Phrase(saveButtonType, [])
let buildLoadButton target = Phrase(loadButtonType, [target])

//let buildConstructDefinition name fieldList meanings =
//    Phrase(ref constructDefinitionPrimitive, [|ref name; ref fieldList; ref meanings|])

let buildConstructDefinition name fieldList =
    Phrase(constructDefinitionPrimitive, [name; fieldList])

let buildDefinitions definitions =
    Phrase(meaningListPrimitive, definitions)
    
//let buildDefinition context meaning =
//    Phrase(definitionType, [context; meaning])

// here we need all the basic primitive entities plus a "field reference" phrase
// to refer back to fields on the phrase we are interpreting
// how do we refer to them? by name? NO! name != idphrase
// direct pointer? to what? the field def? that can't work - how can it find the value
// of a particular instance??  we need a reference abstraction
//let buildMeaning meaning =
   

let buildName name =
    Phrase(namePrimitive, [buildString name])

let buildFields fields =
    Phrase(constructFieldListPrimitive, fields)

let buildSimpleField name fieldType =             /// use name primitive instead of string?
    Phrase(constructFieldPrimitive, [buildName name; fieldType])
    
let buildGenericType rootType typePart =
    Phrase(constructGenericTypePrimitive, [rootType; typePart])

//let buildConstructType construct =
    //Phrase(ref (TypePhrase(constructConstructTypePrimitive)), [|ref (TypeRefPhrase (ref construct))|])

let intFieldType = Phrase(constructFieldTypePrimitive, [intType])
let stringFieldType = Phrase(constructFieldTypePrimitive, [stringType])
let userFieldType userType = Phrase(constructFieldTypePrimitive, [userType])

//let buildListOf itemType =
//    Phrase(ref constructGenericTypePrimitive, [|ref stringFieldType; itemType|])
//stringFieldType //(temporary nonsense) use: listFieldType

let buildRender phrase context = Phrase(renderType, [phrase; context])

let undoButton() = Phrase(undoButtonType, [])
let redoButton() = Phrase(redoButtonType, [])

