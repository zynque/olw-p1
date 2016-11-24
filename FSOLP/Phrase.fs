#light

module FSOLP.Phrase

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives

//-----------------------------------------------------------------------------

type Context = String | WPF | Editor // | Custom of string

type Phrase =
    | StandardPhrase of StandardPhrase
    | Data of PhraseData

and StandardPhrase = {
        index: int
        phraseType: Phrase // TODO: Remove?  (replace with helpers - type is first item in contents)
                           //       This would help with meta-circularity!
        mutable parents: Phrase List
        mutable contents: Phrase List
        mutable interpretations: Map<Context, Phrase>
        mutable quotations: Map<Context, Phrase>
    }
    
and PhraseData =
    | IntPhrase of int
    | StringPhrase of string
    | DisplayablePhrase of DisplayablePrimitive
    | TypePhrase of PhraseType
    | Context of Context

and PhraseType = {
        name: string
        definitionMap: Map<Context, Renderer>
        quoteMap: Map<Context, Renderer>
    }

and Renderer = Phrase -> (RenderContext) -> Phrase
    
and Delta = Delta of Phrase * Phrase List * Phrase List

and Changeset = Delta list

and RenderContext = {
        root: Phrase
        rerender: unit -> unit
        mutable undoStack: Changeset
        mutable redoStack: Changeset
    }

//type PhraseDataItem =
//    {
//        index: int
//        phraseType: int // TODO: Remove?
//        mutable parents: int List
//        mutable contents: int List
//        mutable interpretations: Map<Context, Phrase>
//        mutable quotations: Map<Context, Phrase>
//    }
//            
//type PhraseDataCache = PhraseDataItem array
