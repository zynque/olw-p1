#light

module FSOLP.Render

open FSOLP.Phrase
open FSOLP.Deltas
open FSOLP.PhraseExtractors
open FSOLP.DisplayPrimitives

// ----------------------------------------------------------------------------

let rec quote (phrase: Phrase) (renderContext: RenderContext) (context: Context): Phrase =
    match phrase with
        | StandardPhrase(se) ->
            if not (se.quotations.ContainsKey context) then
                let quotation = quoteStandard phrase renderContext context
                se.quotations <- Map.add context quotation se.quotations
                quotation
            else
                se.quotations.Item context
        | Data(DisplayablePhrase(d)) ->
            match context with
                | WPF -> Data(DisplayablePhrase(d))
                | _ -> failwith("DisplayablePhrase can only be displayed in Wpf Context")
        | Data(TypePhrase(t)) ->
            match context with
                | WPF -> Data(DisplayablePhrase(Text(t.name)))
                | _ -> failwith("TypePhrase can only be displayed in wpf Context")
        | Data(Context(c)) -> 
            match context with
                | String -> Data(StringPhrase(contextAsString c))
                | Editor | WPF -> Data(DisplayablePhrase(Text(contextAsString c)))
        | _ -> failwith("Not Implemented")

and quoteStandard (phrase: Phrase) (renderContext: RenderContext) (context: Context): Phrase =
    match phrase with
        | StandardPhrase(se) ->
            let typeModel = phraseAsType (render se.phraseType renderContext context)
            let quote = getQuoterForContext typeModel context
            quote phrase renderContext
        | _ -> failwith("Fatal Error: StandardPhrase expected")

and getQuoterForContext (phraseType: PhraseType) context: Renderer =
    let possibleDefinition = phraseType.quoteMap.TryFind context
    match possibleDefinition with
        | Some(definition) -> definition
        | None -> 
            failwith(phraseType.name + " is not defined under " + contextAsString context + " for quotation")

//! can this be reduced to the phrase case and the typephrase case only?
//  ie - can all others reference metatypes?
and render (phrase: Phrase) (renderContext: RenderContext) (context: Context): Phrase =
    match phrase with
        | StandardPhrase(se) ->
            if not (se.interpretations.ContainsKey context) then
                //let renderedContext = render context renderContext
                let interpretation = renderStandard phrase renderContext context
                se.interpretations <- Map.add context interpretation se.interpretations
                interpretation
            else
                Map.find context se.interpretations
        | Data(d) -> Data(d) // base case

and renderStandard (phrase: Phrase) (renderContext: RenderContext) (context: Context): Phrase =
    match phrase with
        | StandardPhrase(se) ->
            // assume context phrase is already rendered
            //? let renderedContext = render context Interpreter
            let typeModel = phraseAsType (render se.phraseType renderContext context)//getRendererFromType phraseType context
            let render = getRendererForContext typeModel context
            render phrase renderContext
        | _ -> failwith("Fatal Error: StandardPhrase expected")

and getRendererForContext (phraseType: PhraseType) context: Renderer =
    let possibleDefinition = phraseType.definitionMap.TryFind context
    match possibleDefinition with
        | Some(definition) -> definition
        | None ->
            failwith(phraseType.name + " is not defined under " + contextAsString context + " for rendering")
