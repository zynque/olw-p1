#light

module FSOLP.FileLoader

open FSOLP.Phrase
open FSOLP.Deltas
open FSOLP.PhraseExtractors
open FSOLP.DisplayPrimitives

// ----------------------------------------------------------------------------

let unescapeMatch (m:System.Text.RegularExpressions.Match) =
    match m.ToString() with
    | @"\n" -> "\n"
    | @"\\" -> @"\"
    | @"\""" -> "\""
    | @"\t" -> "\t"
    | s -> s

let unescapeString s =
    let matchEvaluator = new System.Text.RegularExpressions.MatchEvaluator(unescapeMatch)
    System.Text.RegularExpressions.Regex.Replace(s, @"(\\n)|(\\\\)|(\\"")|(\\t)", matchEvaluator)

let (|Regex|_|) (pattern:string) (input:string) =
    let result = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if result.Success then
        Some result.Groups
    else
        None

type PhraseData =
    | StandardData of int list
    | ReferenceData of string
    | StringData of string
    | IntData of int
    
type PhraseEntry = {index: int; data: PhraseData}

let readLine line =
    match line with
    | Regex @"(?<index>\d+):(?:(?<children>\d+),)*(?<lastChild>\d+)$" groups ->
        {
            index = int (groups.["index"]).Value
            data = StandardData
             [
                let childrenCaptures = groups.["children"].Captures
                for i in [0 .. childrenCaptures.Count-1] do
                    yield int childrenCaptures.[i].Value
                yield int groups.["lastChild"].Value             
             ]
        }
    | Regex @"(?<index>\d+)=(?<url>http://.+)$" groups ->
        {
            index = int (groups.["index"]).Value
            data = ReferenceData (groups.["url"]).Value
        }
    | Regex @"(?<index>\d+)=""(?<string>([^\\]|(\\n)|(\\\\)|(\\"")|(\\t))*)""$" groups ->
        {
            index = int (groups.["index"]).Value
            data = StringData (unescapeString (groups.["string"]).Value)
        }
    | Regex @"(?<index>\d+)=(?<int>\d+)" groups ->
        {
            index = int (groups.["index"]).Value
            data = IntData (int (groups.["int"]).Value)
        }
    | _ -> failwith ("OLP Document parser failed to parse line: " + line)

let readFile (fileName:string) =
    [
        use stream = new System.IO.StreamReader(fileName)
        let line = ref (stream.ReadLine())
        while (!line <> null) do
            yield readLine !line
            line := stream.ReadLine()
    ]

let toEntryGraph (entries:PhraseEntry list) =
    let mutable maxIndex = 0
    for entry in entries do
        if entry.index > maxIndex then maxIndex <- entry.index
    let entryGraph = Array.zeroCreate (maxIndex + 1)
    for entry in entries do
        entryGraph.[entry.index] <- entry.data
    entryGraph

let Phrase (eType, contents, index) =
    let newPhrase = StandardPhrase (
                     {
                        index = index
                        phraseType = eType
                        parents = []
                        contents = contents
                        interpretations = Map.empty<Context,Phrase>
                        quotations = Map.empty<Context,Phrase>
                     }
        )
    let addParent = fun phrase ->
        match phrase with
            | StandardPhrase(sp) -> sp.parents <- List.append sp.parents [newPhrase]
            | _ -> ()
    List.iter addParent contents
    newPhrase

let toPhrases (entryGraph: PhraseData array) typePhraseBuilder =

    let mappedPhrases = Array.zeroCreate entryGraph.Length
    
    let rec createItem i =
        if obj.ReferenceEquals(mappedPhrases.[i], null) then
            let phrase =
                match entryGraph.[i] with
                | StandardData(children) ->
                    let childPhrases = List.map createItem children
                    Phrase(childPhrases.Head, childPhrases.Tail, i)
                | ReferenceData(url) -> typePhraseBuilder(url)
                | StringData(s) -> Data(StringPhrase(s))
                | IntData(i) -> Data(IntPhrase(i))
            mappedPhrases.[i] <- phrase
            phrase
        else
            mappedPhrases.[i]
              
    for i in [0 .. entryGraph.Length-1] do
        if not (obj.ReferenceEquals(entryGraph.[i], null)) then
            mappedPhrases.[i] <- createItem i
    
    mappedPhrases

let phraseFromFile file typePhraseBuilder =
    let entries = readFile file
    let rootIndex = entries.[0].index
    let entryGraph = toEntryGraph entries
    let phrases = toPhrases entryGraph typePhraseBuilder
    phrases.[rootIndex]
