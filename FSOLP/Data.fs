#light

module FSOLP.Data

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Tree

//-----------------------------------------------------------------------------

type Context = WPF | ScalaSource // | Custom of string?

type Environment = {Blah: int}

and OLPTree = Node<PhraseData>

and Renderer = OLPTree -> Environment -> OLPTree

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

let render2 tree context environment =
    match tree with
    | {Children = {Data = TypePhrase(t)}::cs } -> t.definitionMap.[context] tree environment
    | t -> t

let quote2 tree context environment =
    match tree with
    | {Children = {Data = TypePhrase(t)}::cs } -> t.quoteMap.[context] tree environment
    | t -> t

let buildWPFType (name, wpfDef, wpfQuote) =
        {
            name = name
            definitionMap = Map.ofList [(WPF,wpfDef)]
            quoteMap = Map.ofList [(WPF,wpfQuote)]
        }

let typeOf tree = tree.Children.Head
let childrenOf tree = tree.Children.Tail
let childOf tree = tree.Children.Tail.Head
let intOf tree = match childOf tree with
                    | {Data = IntPhrase(i)} -> i
                    | _ -> failwith "Expected int phrase"


let intType2 = buildWPFType
                (
                  "Int"
                  ,
                  fun phrase env -> childOf phrase
                  ,
                  fun phrase env ->
                        let value = intOf phrase
                        {Children = []; Data = StringPhrase(value.ToString())}
                )

let cachedType cachedRender cachedQuote =
        buildWPFType
                (
                  "Cached"
                  ,
                  fun phrase env -> cachedRender
                  ,
                  fun phrase env -> cachedQuote
                )
