#light

module FSOLP.PhraseExtractors

open FSOLP.Phrase

// ----------------------------------------------------------------------------

let contextAsString context =
    match context with
    | String -> "String Context"
    | WPF -> "WPF Context"
    | Editor -> "Editor Context"

let typeFail(typeName, obj) = failwith("Expected " + typeName + " but was: " + obj.GetType().ToString())

let phraseAsStandard phrase =
    match phrase with
        | StandardPhrase(e) -> e
        | a -> typeFail("StandardPhrase", a)

let phraseAsInt phrase =
    match phrase with
        | Data(IntPhrase(i)) -> i
        | a -> typeFail("IntPhrase", a)

let phraseAsString phrase =
    match phrase with
        | Data(StringPhrase(s)) -> s
        | a -> typeFail("StringPhrase", a)

let phraseAsDisplayable phrase =
    match phrase with
        | Data(DisplayablePhrase(d)) -> d
        | a -> typeFail("DisplayablePhrase", a)

let phraseAsType phrase =
    match phrase with
        | Data(TypePhrase(t)) -> t
        | a -> typeFail("TypePhrase", a)        

let phraseAsContext phrase =
    match phrase with
        | Data(Context(c)) -> c
        | a -> typeFail("ContextPhrase", a)
