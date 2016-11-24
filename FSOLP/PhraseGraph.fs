#light

module FSOLP.PhraseGraph

open FSOLP.Phrase
open FSOLP.Deltas
open FSOLP.PhraseExtractors
open FSOLP.DisplayPrimitives

// ----------------------------------------------------------------------------

type PhraseData =
    | StandardData of int list
    | ReferenceData of string
    | StringData of string
    | IntData of int
    
type PhraseEntry = {index: int; data: PhraseData}
