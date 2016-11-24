#light

module FSOLP.PhraseSandbox

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.Deltas
open FSOLP.ConstructDefinitionEntities
open FSOLP.PhraseBuilders
open FSOLP.Render
open FSOLP.PhraseExtractors
open FSOLP.Context
open FSOLP.SimpleOperators

//-----------------------------------------------------------------------------

let mockRenderContext =
    {
        root = buildString("MockRoot")
        rerender = ignore
        undoStack = []
        redoStack = []
    }

//-----------------------------------------------------------------------------

let mathExpression = buildSubtract(
                        [
                            buildInt(100)
                            buildInt(50)
                            buildInt(25)
                            buildMultiply(
                                [
                                    buildInt(-3)
                                    buildInt(-4)
                                ])
                        ])
                    
let quotedMathExpression = buildQuote mathExpression

let renderExpression = buildRender quotedMathExpression stringContext
let quotedRenderExpression = buildQuote renderExpression
let renderedQuotedRender = Text( phraseAsString(render quotedRenderExpression mockRenderContext String) )
let renderedRender = Text( phraseAsString(render renderExpression mockRenderContext String) )

let mathExample = Rows ([
                            Box(displayContexts quotedMathExpression mockRenderContext)
                            renderedQuotedRender
                            renderedRender
                            Text(phraseAsInt(render mathExpression mockRenderContext WPF).ToString())
                        ])

//-----------------------------------------------------------------------------

let drawStuff = buildColumns [
                    buildBox (
                        buildColumns [
                            buildString "A"
                            buildString "B"
                            buildString "C"
                        ]
                    )
                    buildArrow
                    buildBox (
                        buildRows [
                            buildString "A"
                            buildString "B"
                            buildString "C"
                        ]
                    )
                ]
        
let drawTest = phraseAsDisplayable (render drawStuff mockRenderContext WPF)

//-----------------------------------------------------------------------------


//let personConstructType =
////    let unresolvedConstructTypeRef = ref NullPhrase
let constDef = buildConstructDefinition
                    (buildName "Person")
                    (buildFields [
                            buildSimpleField "Name" stringFieldType
                            buildSimpleField "Age" intFieldType
                            buildSimpleField "Favorite Color" stringFieldType;
                            //buildSimpleField "Best Friend" (userFieldType (render constDef interpreterContext))
//                                 (
//                                    buildListOf unresolvedConstructTypeRef //!!!inf loop??
//                                 )
                     ])
//                     (
//                        buildDefinitionList
//                            [|
//                                buildDefinition
//                                 (
//                                    wpfContext, 
//                                    buildMeaning
//                                        wpfrender ref name
//                                  )
//                            |]
//                     )

// A construct definition is not a type...
// rendering a construct definition under the interpreter context generates a type

// hidden for now
let personType = render constDef mockRenderContext WPF

let constDef2 = buildConstructDefinition
                    (buildName "Family")
                    (buildFields [
                        buildSimpleField "Name" stringFieldType
                        buildSimpleField "Mother" (userFieldType personType)
                        buildSimpleField "Father" (userFieldType personType)
                        buildSimpleField "Child" (userFieldType personType)
                    ])
//                    //(buildMeaning [||])

let constInst = Phrase(
                     personType,
                     [
                        buildString("bob");
                        buildInt(45);
                        buildString("purple")
                     ]
                    )
                    
//    unresolvedConstructTypeRef := constDef
//    constDef
//    
//let companyConstructType =
//        buildConstructDefinition
//                (buildName "Company")
//                (buildFields [
//                        buildSimpleField "Employees"
//                            (
//                                buildListOf (ref personConstructType)
//                            )
//                    ])

//let renderedConstDef = phraseAsDisplayable (render companyConstructType CoreWpfContext)

let constructTest = Box(
                        Rows[
                                phraseAsDisplayable (render (buildQuote constDef) mockRenderContext WPF)
                                phraseAsDisplayable (render (buildQuote constDef2) mockRenderContext WPF)
                                phraseAsDisplayable (render (buildQuote constInst) mockRenderContext WPF)
                            ]
                       )

//-----------------------------------------------------------------------------

let updateableInt = buildInt 3
let editableInt = buildRender updateableInt editorContext
let attachedExpression = buildAdd [ updateableInt; (buildInt 4)]
let quotedAttachedExpression = buildQuote attachedExpression
let interpretedAttachedExpression = buildRender attachedExpression wpfContext
let displayableInterpretedAttachedExpression = buildIntToEditable interpretedAttachedExpression
let nonRenderedEditableIntExample =
    buildColumns[
                    editableInt
                    buildString(": ")
                    quotedAttachedExpression
                    buildString(" = ")
                    displayableInterpretedAttachedExpression
                ]
let editableIntExample = buildRender nonRenderedEditableIntExample wpfContext

 //editable(value = 3) : {value + 4} = [value + 4]
 
// GOAL: 
let quotedEditableIntExample = buildQuote editableIntExample

let updateableString = buildString "Change me!"
let attachedDiagram = buildColumns[buildString "***"; updateableString; buildString "***"]
let renderedDiagram = buildRender attachedDiagram wpfContext
let editableStringExample = buildColumns[updateableString; renderedDiagram]

let editableExamples =
    buildRows[
                buildColumns[undoButton() ; redoButton()]
                quotedEditableIntExample
                editableIntExample
                editableStringExample
                buildString "blah"
                buildString "blah2"
                buildSaveButton()
             ]

let renderedEditableExamples = RerenderablePanel (rerenderCapture editableExamples)

let loadTarget = buildBox (buildString "[...]")
let loader = buildBox (
                buildRows [
                    buildLoadButton loadTarget
                    loadTarget
                ]
             )
             
let loadTest = RerenderablePanel (rerenderCapture loader)

let phraseTest = ScrollPanel (
                     Rows[
                         renderedEditableExamples
                         HorizontalRule
                         constructTest
                         HorizontalRule
                         mathExample
                         HorizontalRule
                         drawTest
                         HorizontalRule
                         loadTest
                         Box (FocusableList [Focusable "hi";Focusable "ho";Focusable "hum"])
                     ])
