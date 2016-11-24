#light

module FSOLP.Context

open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render

// ----------------------------------------------------------------------------

let contextType =
        TypePhrase {
        name = "ContextType";
        definitionMap = Map.ofList
            [
//                (
//                    Interpreter,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let contextName = phraseAsString se.contents.Head
//                        if se.contents.Tail.IsEmpty
//                            then Data(Context context)
//                            else se.contents.Tail.Head // primitive contexts
//                )
                (
                    String,
                    fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let contextName = phraseAsString se.contents.Head
                        Data(StringPhrase contextName)
                )
                (
                    WPF,
                    fun phrase rerender -> 
                        let se = phraseAsStandard phrase
                        let contextName = phraseAsString se.contents.Head
                        Data(DisplayablePhrase(Text contextName))
                )
            ]
        quoteMap = Map.ofList
            [
//                (
//                    interpreterContext,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let contextName = phraseAsString se.contents.Head
//                        if se.contents.Tail.IsEmpty
//                            then ContextPhrase (contextName)
//                            else se.contents.Tail.Head // primitive contexts
//                )
                (
                    String,
                    fun phrase rerender ->
                        let se = phraseAsStandard phrase
                        let contextName = phraseAsString se.contents.Head
                        Data(StringPhrase contextName)
                )
                (
                    WPF,
                    fun phrase rerender -> 
                        let se = phraseAsStandard phrase
                        let contextName = phraseAsString se.contents.Head
                        Data(DisplayablePhrase(Text contextName))
                )
            ]
    }

//let stringContext2 =
//    StandardPhrase (
//        {
//            phraseType = contextType
//            parents = []
//            contents = [StringPhrase String; stringContext]
//            interpretations = Map.empty<Phrase,Phrase>
//            quotations = Map.empty<Phrase,Phrase>
//        }
//    )
//    
//let interpreterContext2 =
//    StandardPhrase (
//        {
//            phraseType = contextType
//            parents = []
//            contents = [StringPhrase Interpreter; interpreterContext]
//            interpretations = Map.empty<Phrase,Phrase>
//            quotations = Map.empty<Phrase,Phrase>
//        }
//    )
//
//let wpfContext2 =
//    StandardPhrase (
//        {
//            phraseType = contextType
//            parents = []
//            contents = [StringPhrase WPF; wpfContext]
//            interpretations = Map.empty<Phrase,Phrase>
//            quotations = Map.empty<Phrase,Phrase>
//        }
//    )
//
//let editorContext2 =
//    StandardPhrase (
//        {
//            phraseType = contextType
//            parents = []
//            contents = [StringPhrase Interpreter; Editor]
//            interpretations = Map.empty<Phrase,Phrase>
//            quotations = Map.empty<Phrase,Phrase>
//        }
//    )
   
//let definitionType =
//    Data(TypePhrase {
//        name = "DefinitionType";
//        definitionMap = Map.ofList
//            [
//                (
//                    Interpreter,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let c = phraseAsContext se.contents.Head
//                        let d = phraseAsRenderer se.contents.Tail.Head // assumes primitives only
//                        Data(DefinitionWithContextPhrase {context = c; definition = d})
//                )
//            ] 
//        quoteMap = Map.ofList
//            [
//                (
//                    Interpreter,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let c = phraseAsContext se.contents.Head
//                        let d = phraseAsRenderer se.contents.Tail.Head // assumes primitives only
//                        Data(DefinitionWithContextPhrase {context = c; definition = d})
//                )
//            ] 
//    })
   
// FUTURE
// this is a "close the loop" point - renderer can be user defined or point to core implimentation
//   ... or a circular implementation
//let rendererType =
//    {
//        name = "RendererType";
//        definitions =
//            [
//                {
//                    context = interpreterContext;
//                    renderer = fun phrase children ->
//                }
//            ] 
//    }
