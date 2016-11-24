#light

module FSOLP.LoadButtonType

open System
open System.Windows
open System.Windows.Controls
open FSOLP.DisplayPrimitives
open FSOLP.Phrase
open FSOLP.PhraseExtractors
open FSOLP.Render
open FSOLP.Context
open FSOLP.PhraseHelpers
open FSOLP.Deltas
open FSOLP.DisplayPrimitives
open FSOLP.ConstructDefinitionEntities
open FSOLP.SimpleTypes
open FSOLP.SimpleOperators
open FSOLP.DisplayableEntities
open FSOLP.RenderQuoteTypes
open FSOLP.FileWriter
open FSOLP.FileLoader

//-----------------------------------------------------------------------------

let saveButtonType =
    generatePrimitiveTypePhrase
        {
            name = "SaveButton"
            definitionList = 
                [
                    {
                        context = WPF;
                        definition = fun phrase renderContext ->
                            let writeToFile (content:string) (filename:string) =
                                let writer = new System.IO.StreamWriter(filename)
                                writer.Write(content)
                                writer.Close()
                            let action = fun filename ->
                                let renderedContent = Data(StringPhrase(renderToFile renderContext.root 0))
                                let stringContent = phraseAsString renderedContent
                                writeToFile stringContent filename
                            Data(DisplayablePhrase(SaveFileButton("Save To File", action)))
                    }
                    {
                        context = Editor;
                        definition = fun phrase renderContext ->
                            let writeToFile (content:string) (filename:string) =
                                let writer = new System.IO.StreamWriter(filename)
                                writer.Write(content)
                                writer.Close()
                            let action = fun filename ->
                                let renderedContent = Data(StringPhrase(renderToFile renderContext.root 0))
                                let stringContent = phraseAsString renderedContent
                                writeToFile stringContent filename
                            Data(DisplayablePhrase(SaveFileButton("Save To File", action)))
                    }
                ]
            quoteList = []
        }

// TODO: replace with automatic generation of this mapping
let rec phraseType id =
    match id with
    | "http://olpprimitives/Rows" -> rowsPrimitive
    | "http://olpprimitives/Columns" -> columnsPrimitive
    | "http://olpprimitives/UndoButton" -> undoButtonType
    | "http://olpprimitives/RedoButton" -> redoButtonType
    | "http://olpprimitives/Quote" -> quoteType
    | "http://olpprimitives/Render" -> renderType
    | "http://olpprimitives/Int" -> intType
    | "http://olpprimitives/Editor Context" -> Data(Context(Editor))
    | "http://olpprimitives/String" -> stringType
    | "http://olpprimitives/Add" -> addPrimitive
    | "http://olpprimitives/IntToEditable" -> intToEditableType
    | "http://olpprimitives/WPF Context" -> Data(Context(WPF))
    | "http://olpprimitives/SaveButton" -> saveButtonType
    | "http://olpprimitives/LoadButton" -> loadButtonType // (mutually recursive)
    | "http://olpprimitives/Box" -> boxPrimitive
    | url -> failwith ("Unknown phrase: " + url)


and loadButtonType =
    generatePrimitiveTypePhrase
        {
            name = "LoadButton"
            definitionList = 
                [
//                    {
//                        context = wpfContext;
//                        definition = fun phrase renderContext ->
//                            let writeToFile (content:string) (filename:string) =
//                                let writer = new System.IO.StreamWriter(filename)
//                                writer.Write(content)
//                                writer.Close()
//                            let action = fun filename ->
//                                let renderedContent = render renderContext.root renderContext File
//                                let stringContent = phraseAsString renderedContent
//                                writeToFile stringContent filename
//                            DisplayablePhrase(SaveFileButton("Save To File", action))
//                    }
                    {
                        context = Editor;
                        definition = fun phrase renderContext ->
                            let sp = phraseAsStandard phrase
                            let targetContainer = sp.contents.Head
                            let targetContainerSP = phraseAsStandard targetContainer
                            let loadFromFile (filename:string) =
                                let reader = new System.IO.StreamReader(filename)
                                reader.ReadToEnd()
                            let action = fun filename ->
                                let newContent = phraseFromFile filename phraseType
                                let ncsp = phraseAsStandard newContent
                                ncsp.parents <- phrase :: ncsp.parents
                                let delta = Delta(targetContainer, targetContainerSP.contents, [newContent])
                                renderContext.undoStack <- delta :: renderContext.undoStack
                                renderContext.redoStack <- []
                                applyDelta delta
                                renderContext.rerender() |> ignore
                            Data(DisplayablePhrase(LoadFileButton("Load From File", action)))
                    }
                ]
            quoteList = []
        }
