#light

module FSOLP.FileWriter

open FSOLP.Phrase
open FSOLP.Deltas
open FSOLP.PhraseExtractors
open FSOLP.DisplayPrimitives
open FSOLP.PhraseGraph

// ----------------------------------------------------------------------------

let escapeMatch (m:System.Text.RegularExpressions.Match) =
    match m.ToString() with
    | "\n" -> @"\n"
    | @"\" -> @"\\"
    | "\"" -> @"\"""
    | "\t" -> @"\t"
    | s -> s

let escapeString (s:string) =
    let matchEvaluator = new System.Text.RegularExpressions.MatchEvaluator(escapeMatch)
    System.Text.RegularExpressions.Regex.Replace(s, @"\n|\\|""|\t", matchEvaluator)

let writeEntry phraseEntry =
    let i = phraseEntry.index
    let d = phraseEntry.data
    match d with
    | StandardData is ->
        let stringInts = List.map (fun (i) -> i.ToString()) is
        let commaSeparated = List.reduce (fun sa sb -> sa + "," + sb) stringInts
        i.ToString() + ":" + commaSeparated
    | ReferenceData url ->
        i.ToString() + "=" + url
    | StringData s ->
        i.ToString() + "=" + "\"" + (escapeString s) + "\""
    | IntData d ->
        i.ToString() + "=" + d.ToString()

type PhraseGraphBuilder(root: Phrase) =
 class
    let mutable maxId = 0
    let mutable nextId = 0
    let mutable indexedPhrases = Map.empty<int, PhraseData>
    let mutable mappedReferences = Map.empty<string, int>
    let mutable mappedInts = Map.empty<int, int>
    let mutable mappedStrings = Map.empty<string, int>

    let addMappedReferences () =
        let addRef ref index =
            indexedPhrases <- Map.add index (ReferenceData(ref)) indexedPhrases 
        Map.iter addRef mappedReferences
        
    let addMappedInts () =
        let addInt i index =
            indexedPhrases <- Map.add index (IntData(i)) indexedPhrases 
        Map.iter addInt mappedInts

    let addMappedStrings () =
        let addString s index =
            indexedPhrases <- Map.add index (StringData(s)) indexedPhrases 
        Map.iter addString mappedStrings

    let nextIndex () =
        let index = nextId
        nextId <- nextId + 1
        index

    let rec getMaxIndex (phrase:Phrase): int =
        match phrase with
            | StandardPhrase(sp) ->
                let index = sp.index
                let typeIndex = getMaxIndex sp.phraseType
                let maxChild =
                    if List.isEmpty sp.contents then
                        0
                    else
                        let childIndices = List.map getMaxIndex sp.contents
                        List.max childIndices
                List.max [index; typeIndex; maxChild]
            | _ -> 0
            
    let rec generatePhraseGraph (phrase:Phrase): int =
             match phrase with
                | StandardPhrase(sp) ->
                    let index = sp.index
                    if indexedPhrases.ContainsKey index then
                        index
                    else
                        let typeId = generatePhraseGraph sp.phraseType
                        let childIds = List.map generatePhraseGraph sp.contents
                        let data = StandardData (typeId::childIds)
                        indexedPhrases <- indexedPhrases.Add (index, data)
                        index
                | Data(TypePhrase(t)) ->
                    let reference = "http://olpprimitives/" + t.name
                    if mappedReferences.ContainsKey reference then
                        mappedReferences.Item reference
                    else
                        let index = nextIndex()
                        mappedReferences <- mappedReferences.Add (reference, index)
                        index
                | Data(IntPhrase(i)) ->
                    if mappedInts.ContainsKey i then
                        mappedInts.Item i
                    else
                        let index = nextIndex()
                        mappedInts <- mappedInts.Add (i, index)
                        index
                | Data(StringPhrase(s)) ->
                    if mappedStrings.ContainsKey s then
                        mappedStrings.Item s
                    else
                        let index = nextIndex()
                        mappedStrings <- mappedStrings.Add (s, index)
                        index
                | Data(Context(c)) ->
                    let reference = "http://olpprimitives/" + contextAsString c
                    if mappedReferences.ContainsKey reference then
                        mappedReferences.Item reference
                    else
                        let index = nextIndex()
                        mappedReferences <- mappedReferences.Add (reference, index)
                        index
                | _ -> failwith("This type of phrase should never be rendered to file")
            
    let generateArray () =
        let maxUsedIndex = getMaxIndex root
        nextId <- maxUsedIndex + 1
        generatePhraseGraph root |> ignore
        addMappedReferences()
        addMappedInts()
        addMappedStrings()
        let array: PhraseEntry array = Array.zeroCreate nextId
        let addToArray i d = array.[i] <- {index = i; data = d}
        Map.iter addToArray indexedPhrases
        let rootIndex =
            match root with
            | StandardPhrase(sp) -> sp.index
            | _ -> failwith "Expected standard phrase at root of graph"
        let firstEntry = array.[0]
        let rootEntry = array.[rootIndex]
        array.[0] <- rootEntry
        array.[rootIndex] <- firstEntry
        array
    
    member x.render = 
        let array = generateArray()
        let renderEntry entry =
            if obj.ReferenceEquals(entry, null) then
                ""
            else
                writeEntry entry
        let renderedData = Array.map renderEntry array
                            |> Array.filter (fun s -> not (System.String.IsNullOrEmpty s))
        System.String.Join("\n",renderedData)
        
    end

let renderToFile (phrase: Phrase) (tabs: int): string =
    let builder = new PhraseGraphBuilder(phrase)
    builder.render

let renderToFile2 (phrase: Phrase) (tabs: int): string =
    let tabstring = System.String.Join("", List.toArray(List.init tabs (fun n -> "\t")))
    match phrase with
        | StandardPhrase(sp) ->
            let phraseType = renderToFile sp.phraseType 0
            if sp.contents.Length = 0 then
                tabstring + phraseType
            else
                let renderChild = fun (phrase) -> renderToFile phrase (tabs + 1)
                let children = List.map renderChild sp.contents
                tabstring + phraseType + "\n" +
                 System.String.Join("\n",List.toArray(children))
        | Data(TypePhrase(t)) ->
            tabstring + t.name
        | Data(IntPhrase(i)) ->
            tabstring + i.ToString()
        | Data(StringPhrase(s)) ->
            tabstring + s
        | Data(Context(c)) ->
            tabstring + contextAsString c
        | _ -> failwith("This type of phrase should never be rendered to file")
