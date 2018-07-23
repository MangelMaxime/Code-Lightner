module CodeLightner

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open VSCode.TextMate
open Fable.Import
open System
open Helpers

let private colors = ColorJS.safe

let private resolve (path : string) =
    let segments =
        [| Node.``process``.cwd()
           path
        |]
        |> ResizeArray
    Node.path.resolve segments

let private loadThemeFile (filename : string) =
    let filePath = resolve filename
    if Node.fs.existsSync(unbox<Node.Fs.PathLike> filePath) then
        let content = Node.fs.readFileSync(unbox<U2<Node.Fs.PathLike,float>> (resolve filename), Some null).toString()

        if Node.path.extname filename = ".json" then
            JS.JSON.parse(content) :?> IRawTheme
        else
            ``fast-plist``.parse(content)
        |> Some
    else
        JS.console.warn(colors.yellow.Invoke("Theme file not found: " + filePath))
        None

[<RequireQualifiedAccess>]
type FontStyle =
    | NotSet = -1
    | None = 0
    | Italic = 1
    | Bold = 2
    | Underline = 4

type StackElementMetadata =
    abstract getLanguageId : metadata : float -> int
    abstract getTokenType : metadata : float -> StandardTokenType
    abstract getFontStyle : metadata : float -> FontStyle
    abstract getForeground : metadata : float -> int
    abstract getBackground : metadata : float -> int

let private tokenMetadataDecoder : StackElementMetadata = import "StackElementMetadata" "./js/helpers.js"

let private escapeHtml (_ : string) : string = import "escapeHtml" "./js/helpers.js"

let private getInlineStyleFromMetadata (metadata : float) (colorMap :  ResizeArray<string>) : string =
    let foreground = tokenMetadataDecoder.getForeground(metadata)
    let fontStyle = tokenMetadataDecoder.getFontStyle(metadata)

    let foregroundCSS =
        let color = colorMap.[foreground]
        if not (isNull color) && color <> "#000000" then
            "color: " +  color
        else
            ""

    let fontStyleCSS =
        match fontStyle with
        | FontStyle.Italic -> "font-style: italic"
        | FontStyle.Bold -> "font-style: bold"
        | FontStyle.Underline -> "font-style: underline"
        | FontStyle.NotSet
        | FontStyle.None
        | _ -> ""

    [ foregroundCSS
      fontStyleCSS ]
    |> List.filter (String.IsNullOrWhiteSpace >> not)
    |> String.concat ";"

let rec private lineToHtml (grammar : IGrammar) (colorMap : ResizeArray<string>) (prevState : StackElement) (line : string) =
    let mutable html = ""
    let lineResult = grammar.tokenizeLine2(line, prevState)

    let tokensLength = (int lineResult.tokens.length) / 2

    for i = 0 to tokensLength do
        let startIndex = lineResult.tokens.Item(2 * i)
        let nextStartIndex =
            if i + 1 < tokensLength then
                lineResult.tokens.Item(2 * i + 2)
            else
                float line.Length

        let tokenText =
            line.Substring(int startIndex, int (nextStartIndex - startIndex))
            |> escapeHtml
        if tokenText = "" then
            ()
        else
            let metadata = lineResult.tokens.[2 * i + 1]
            let inlineStyle = getInlineStyleFromMetadata metadata colorMap
            if inlineStyle = "" then
                html <- html + "<span>" + tokenText + "</span>"
            else
                html <- html + "<span style=\"" + inlineStyle + "\">" + tokenText + "</span>"

    html , lineResult.ruleStack

type [<AllowNullLiteral>] IGrammarFiles =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: selector: string -> IRawGrammar with get, set

let private createGrammarFiles (files : (string * string) list) =
        files
        |> List.map (fun (keyName, filePath) ->
            keyName ==> filePath
        )
        |> createObj
        |> unbox<IGrammarFiles>

[<Pojo>]
type CodeToHtmlConfig =
    { backgroundColor : string option
      textColor : string option
      grammarFiles : string list
      codeElementClass : string option
      themeFile : string
      scopeName : string }

let private toInlineStyle (propertyName : string) (optionalValue : string option) =
    match optionalValue with
    | Some color -> propertyName + ": " + color
    | None -> ""

let private loadGrammars (files : string list) =
    promise {
        let mapScopeName (map : Map<string, string>) (file : string) =
            let filePath = resolve file
            if Node.fs.existsSync(unbox<Node.Fs.PathLike> filePath) then
                let content : Node.Buffer= Node.fs.readFileSync(unbox<U2<Node.Fs.PathLike,float>> filePath, Some null)
                let grammar = vsctm.parseRawGrammar (content.toString()) file
                Map.add grammar.scopeName file map
            else
                JS.console.warn(colors.yellow.Invoke("File not found: " + filePath))
                map

        return List.fold mapScopeName Map.empty files
    }

let lighten (config : CodeToHtmlConfig) (code : string) =
    JS.console.log(colors.grey.Invoke("CWD: " + Node.``process``.cwd()))
    promise {
        let! grammars = loadGrammars config.grammarFiles

        let registryOptions = jsOptions<RegistryOptions>(fun o ->
            o.loadGrammar <- (fun scopeName ->
                match grammars.TryFind scopeName with
                | Some filePath ->
                    promise {
                        let content : Node.Buffer = Node.fs.readFileSync(unbox<U2<Node.Fs.PathLike,float>> (resolve filePath), Some null)
                        let rawGrammar = vsctm.parseRawGrammar (content.toString()) filePath
                        return Some rawGrammar
                    }
                | None ->
                    promise {
                        return None
                    }
                |> Promise.toThenable
            )
            o.theme <- loadThemeFile config.themeFile
        )

        let registry = vsctm.Registry.Create(registryOptions)

        return!
            registry.loadGrammar(config.scopeName)
            |> Promise.fromThenable
            |> Promise.map (fun grammar ->
                let colorMap = registry.getColorMap()

                let mutable prevStack = vsctm.INITIAL
                let mutable html = ""
                for line in code.Split('\n') do
                    let (lineHtml, ruleStack) = lineToHtml grammar colorMap prevStack line
                    html <- html + lineHtml + "\n"
                    prevStack <- ruleStack
                html
            )
            |> Promise.map (fun htmlCode ->
                let preStyle =
                    [ toInlineStyle "background-color" config.backgroundColor
                      toInlineStyle "color" config.textColor
                      "padding: 1em" ]
                    |> List.filter (String.IsNullOrEmpty >> not)
                    |> String.concat ";"

                let openPreTag =
                    if preStyle <> "" then
                        "<pre style=\"" + preStyle + "\">"
                    else
                        "<pre>"

                let codeStyle =
                    match config.codeElementClass with
                    | Some style -> "style= \"" + style + "\""
                    | None -> ""

                openPreTag + "<code" + codeStyle + ">" + (htmlCode.Trim()) + "</code></pre>"
            )
    }
    |> Promise.catch(fun error ->
        JS.console.error error
        ""
    )
