#light

module FSOLP.MetaType

open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render

// ----------------------------------------------------------------------------

//let metaType =
//    TypePhrase {
//        name = "TypeType";
//        definitionMap = Map.ofList
//            [
//                (
//                    interpreterContext,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let typeName = phraseAsString se.contents.Head
//                        let extractor = fun int -> phraseAsType (render int rerender interpreterContext)
//                        let defWithContext = List.map extractor se.contents.Tail
//                        TypePhrase {name = typeName; definitions = Map.ofList defWithContext}
//                )
//                (
//                    stringContext,
//                    fun phrase rerender -> 
//                        let se = phraseAsStandard phrase
//                        let typeName = phraseAsString se.contents.Head
//                        StringPhrase typeName
//                )
//                (
//                    wpfContext,
//                    fun phrase rerender ->
//                        let se = phraseAsStandard phrase
//                        let typeName = phraseAsString se.contents.Head
//                        // definitions - an opportunity for "drilling in"
//                        //let intr = List.map (fun int -> phraseAsDefinition (render int interpreterContext)) children.Tail
//                        DisplayablePhrase(Text typeName)
//                )
//            ] 
//    }
