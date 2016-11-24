#light

module FSOLP.ConstructDefinitionEntities

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

//phrase
//    "type"
//        list of: meanings/definitions each of which has:
//            type: "definition" type
//            associated context label
//            interpreter function of phrase -> phrase
//    data (list of entities)

// Add(a: Exp<int>, b: Exp<int>): Exp<int> --> {a} + {b}        <interpet>
//                                             {a} + '"' + {b}  <displ>

//phrase
//    "type"
//        list of: meanings/definitions each of which has:
//            type: "definition" type
//            associated context label
//            interpreter function of phrase -> phrase
//    data (list of entities)

// Add(a: Exp<int>, b: Exp<int>): Exp<int> --> {a} + {b}        <interpet>
//                                             {a} + '"' + {b}  <displ>


let generateUserConstructType constructName =
    generatePrimitiveTypePhrase
        {
            name = constructName;
            definitionList = 
                [
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            Data(DisplayablePhrase(Text(constructName)))
                    }
                ]
        }

let constructDefinitionPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "ConstructDefinition";
            definitionList = 
                [
                    // "Interpreting" a construct definition means converting to a type checking machine
                    {
                        // cache or compile the definition here?
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let name = phraseAsString(render se.contents.Head rerender String)
                            //let meanings = render children.Tail.Tail.Head Interpreter
                            generateUserConstructType name //meanings
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let displayedName = phraseAsDisplayable(render se.contents.Head rerender WPF)
                            let displayedFields = phraseAsDisplayable(render se.contents.Tail.Head rerender WPF)
                            Data(DisplayablePhrase(Box(Rows([displayedName; HorizontalRule ; displayedFields]))))
                    }
                ]
        }

let meaningListPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "MeaningList";
            definitionList = 
                [
//                    {
//                        context = wpfContext;
//                        renderer = fun phrase children ->
//                            let displayedName = phraseAsDisplayable(render children.Head wpfContext)
//                            let displayedFields = phraseAsDisplayable(render children.Tail.Head wpfContext)
//                            DisplayablePhrase(Box(Rows([displayedName; HorizontalRule ; displayedFields])))
//                    }
//                    {
//                        // "Interpreting" meanings
//                        context = Interpreter;
//                        definition = fun phrase rerender ->
//                            let se = phraseAsStandard phrase
//                            //let meanings = List.map(fun child -> render child Interpreter)
//                            //generateMeanings meanings
//                            Data(StringPhrase("TODO"))
//                    }
                ]
            quoteList =
                [
//                    {
//                        context = wpfContext;
//                        renderer = fun phrase children ->
//                            let displayedName = phraseAsDisplayable(render children.Head wpfContext)
//                            let displayedFields = phraseAsDisplayable(render children.Tail.Head wpfContext)
//                            DisplayablePhrase(Box(Rows([displayedName; HorizontalRule ; displayedFields])))
//                    }
//                    {
//                        // "Interpreting" meanings
//                        context = Interpreter;
//                        definition = fun phrase rerender ->
//                            let se = phraseAsStandard phrase
//                            //let meanings = List.map(fun child -> render child Interpreter)
//                            //generateMeanings meanings
//                            Data(StringPhrase("TODO"))
//                    }
                ]
        }

let namePrimitive =
    generatePrimitiveTypePhrase
        {
            name = "Name";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            render se.contents.Head rerender WPF
                    }
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            render se.contents.Head rerender String
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            render se.contents.Head rerender WPF
                    }
                    {
                        context = String;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            render se.contents.Head rerender String
                    }
                ]
        }

let constructFieldListPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "FieldList";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable(render child rerender WPF)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Rows(displayableChildren)))
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let extractor = fun child -> phraseAsDisplayable(render child rerender WPF)
                            let displayableChildren = List.map extractor se.contents
                            Data(DisplayablePhrase(Rows(displayableChildren)))
                    }
                ]
        }

//// interpreter context on a construct definition should generate an phrasetype
//// an phrase type is a collection of contexts on a construct
let constructFieldPrimitive =
    generatePrimitiveTypePhrase
        {
            name = "Field";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let fieldName = phraseAsDisplayable(render se.contents.Head rerender WPF)
                            let fieldType = phraseAsDisplayable(render se.contents.Tail.Head rerender WPF)
                            Data(DisplayablePhrase(Columns([Text("Field: ") ; fieldName ; Text(" [") ; fieldType ; Text("]")])))
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let fieldName = phraseAsDisplayable(render se.contents.Head rerender WPF)
                            let fieldType = phraseAsDisplayable(render se.contents.Tail.Head rerender WPF)
                            Data(DisplayablePhrase(Columns([Text("Field: ") ; fieldName ; Text(" [") ; fieldType ; Text("]")])))
                    }
                ]
        }

let constructFieldTypePrimitive =
    generatePrimitiveTypePhrase
        {
            name = "FieldType";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let typeContent = quote se.contents.Head rerender WPF
                            Data(DisplayablePhrase(Columns([Text("FieldType: ");phraseAsDisplayable(typeContent)])))
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            let se = phraseAsStandard phrase
                            let typeContent = quote se.contents.Head rerender WPF
                            Data(DisplayablePhrase(Columns([Text("FieldType: ");phraseAsDisplayable(typeContent)])))
                    }
                ]
        }

let constructGenericTypePrimitive =
    generatePrimitiveTypePhrase
        {
            name = "GenericType";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            //let rootType = phraseAsType children.Head
                            //let childType = phraseAsType children.Tail.Head
                            //DisplayablePhrase(Columns([Text(rootType.name) ; Text(" of ") ; Text(childType.name)]))
                            Data(DisplayablePhrase(Text("SomeGenericType?")))
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            //let rootType = phraseAsType children.Head
                            //let childType = phraseAsType children.Tail.Head
                            //DisplayablePhrase(Columns([Text(rootType.name) ; Text(" of ") ; Text(childType.name)]))
                            Data(DisplayablePhrase(Text("SomeGenericType?")))
                    }
                ]
        }

let constructConstructTypePrimitive =
    generatePrimitiveTypePhrase
        {
            name = "ConstructBasedType";
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            //let constructDefinition = children.Head
                            //match constructDefinition with
                            //    | Phrase(phrase, children) ->
                            //          let name = children.Head
                            //          let displayedName = phraseAsDisplayable name
                            //let displayableChildren = List.map (fun child -> phraseAsDisplayable child) children
                            //let displayedName = displayableChildren.Head
                            //let displayedFields = displayableChildren.Tail.Head
                            //DisplayablePhrase(Rows([displayedName; HorizontalRule ; displayedFields]))
                            Data(DisplayablePhrase(Text("(construct based type)")))
                    }
                ]
            quoteList =
                [
                    {
                        context = WPF;
                        definition = fun phrase rerender ->
                            //let constructDefinition = children.Head
                            //match constructDefinition with
                            //    | Phrase(phrase, children) ->
                            //          let name = children.Head
                            //          let displayedName = phraseAsDisplayable name
                            //let displayableChildren = List.map (fun child -> phraseAsDisplayable child) children
                            //let displayedName = displayableChildren.Head
                            //let displayedFields = displayableChildren.Tail.Head
                            //DisplayablePhrase(Rows([displayedName; HorizontalRule ; displayedFields]))
                            Data(DisplayablePhrase(Text("(construct based type)")))
                    }
                ]
        }
        
        