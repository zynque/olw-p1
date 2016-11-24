#light

module FSOLP.Deltas

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase

//-----------------------------------------------------------------------------

let isDirty sp = sp.interpretations.IsEmpty

let rec makeDirty (phrase: Phrase) =
    match phrase with
        | StandardPhrase(sp) -> makeDirtySp sp
        | _ -> ()
        
and makeDirtySp (sp: StandardPhrase) =
    if not (isDirty sp) then
        sp.interpretations <- Map.empty<Context,Phrase>
        sp.quotations <- Map.empty<Context,Phrase>
        List.iter makeDirty sp.contents
        List.iter makeDirty sp.parents
    else
        ()

let applyDelta delta =
    match delta with
        | Delta(StandardPhrase(sp), contentsB4, contentsAfter) ->
            sp.contents <- contentsAfter
            makeDirtySp sp
        | _ -> failwith("Deltas can only be applied to standard phrases")

let unapplyDelta delta =
    match delta with
        | Delta(StandardPhrase(sp), contentsB4, contentsAfter) ->
            sp.contents <- contentsB4
            makeDirtySp sp
        | _ -> failwith("Deltas can only be applied to standard phrases")

let applyChangeset changeset = List.iter applyDelta changeset
let unapplyChangeset changeset = List.iter unapplyDelta changeset
