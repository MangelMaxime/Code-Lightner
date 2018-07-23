// ts2fable 0.6.1
module rec VSCode.TextMate

open System
open Fable.Core
open Fable.Import.JS
module Types = VSCode.TextMate.Types

type IRawGrammar = Types.IRawGrammar
type IOnigLib = Types.IOnigLib

let [<Import("*", "vscode-textmate")>] vsctm : IExports = jsNative

type [<AllowNullLiteral>] IExports =
    abstract Registry: RegistryStatic
    abstract parseRawGrammar: (string -> string -> IRawGrammar)
    abstract INITIAL: StackElement

/// A single theme setting.
type [<AllowNullLiteral>] IRawThemeSetting =
    abstract name: string option
    abstract scope: U2<string, ResizeArray<string>> option
    abstract settings: obj

/// A TextMate theme.
type [<AllowNullLiteral>] IRawTheme =
    abstract name: string option
    abstract settings: ResizeArray<IRawThemeSetting>

type [<AllowNullLiteral>] Thenable<'T> =
    inherit PromiseLike<'T>

/// A registry helper that can locate grammar file paths given scope names.
type [<AllowNullLiteral>] RegistryOptions =
    abstract theme: IRawTheme option with get, set
    abstract loadGrammar: (string -> Thenable<IRawGrammar option>) with get, set
    abstract getInjections: (string -> ResizeArray<string>) with get, set
    abstract getOnigLib: (unit -> Thenable<IOnigLib>) with get, set

/// A map from scope name to a language id. Please do not use language id 0.
type [<AllowNullLiteral>] IEmbeddedLanguagesMap =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: scopeName: string -> float with get, set

/// A map from selectors to token types.
type [<AllowNullLiteral>] ITokenTypeMap =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: selector: string -> StandardTokenType with get, set

type [<RequireQualifiedAccess>] StandardTokenType =
    | Other = 0
    | Comment = 1
    | String = 2
    | RegEx = 4

type [<AllowNullLiteral>] IGrammarConfiguration =
    abstract embeddedLanguages: IEmbeddedLanguagesMap option with get, set
    abstract tokenTypes: ITokenTypeMap option with get, set

/// The registry that will hold all grammars.
type [<AllowNullLiteral>] Registry =
    abstract _locator: obj
    abstract _syncRegistry: obj
    /// Change the theme. Once called, no previous `ruleStack` should be used anymore.
    abstract setTheme: theme: IRawTheme -> unit
    /// Returns a lookup array for color ids.
    abstract getColorMap: unit -> ResizeArray<string>
    /// Load the grammar for `scopeName` and all referenced included grammars asynchronously.
    /// Please do not use language id 0.
    abstract loadGrammarWithEmbeddedLanguages: initialScopeName: string * initialLanguage: float * embeddedLanguages: IEmbeddedLanguagesMap -> Thenable<IGrammar>
    /// Load the grammar for `scopeName` and all referenced included grammars asynchronously.
    /// Please do not use language id 0.
    abstract loadGrammarWithConfiguration: initialScopeName: string * initialLanguage: float * configuration: IGrammarConfiguration -> Thenable<IGrammar>
    /// Load the grammar for `scopeName` and all referenced included grammars asynchronously.
    abstract loadGrammar: initialScopeName: string -> Thenable<IGrammar>
    abstract _loadGrammar: initialScopeName: obj * initialLanguage: obj * embeddedLanguages: obj * tokenTypes: obj -> unit
    /// Adds a rawGrammar.
    abstract addGrammar: rawGrammar: IRawGrammar * ?injections: ResizeArray<string> * ?initialLanguage: float * ?embeddedLanguages: IEmbeddedLanguagesMap -> Thenable<IGrammar>
    /// Get the grammar for `scopeName`. The grammar must first be created via `loadGrammar` or `addGrammar`.
    abstract grammarForScopeName: scopeName: string * ?initialLanguage: float * ?embeddedLanguages: IEmbeddedLanguagesMap * ?tokenTypes: ITokenTypeMap -> Thenable<IGrammar>

/// The registry that will hold all grammars.
type [<AllowNullLiteral>] RegistryStatic =
    [<Emit "new $0($1...)">] abstract Create: ?locator: RegistryOptions -> Registry

/// A grammar
type [<AllowNullLiteral>] IGrammar =
    /// Tokenize `lineText` using previous line state `prevState`.
    abstract tokenizeLine: lineText: string * prevState: StackElement -> ITokenizeLineResult
    /// Tokenize `lineText` using previous line state `prevState`.
    /// The result contains the tokens in binary format, resolved with the following information:
    ///   - language
    ///   - token type (regex, string, comment, other)
    ///   - font style
    ///   - foreground color
    ///   - background color
    /// e.g. for getting the languageId: `(metadata & MetadataConsts.LANGUAGEID_MASK) >>> MetadataConsts.LANGUAGEID_OFFSET`
    abstract tokenizeLine2: lineText: string * prevState: StackElement -> ITokenizeLineResult2

type [<AllowNullLiteral>] ITokenizeLineResult =
    abstract tokens: ResizeArray<IToken>
    /// The `prevState` to be passed on to the next line tokenization.
    abstract ruleStack: StackElement

[<RequireQualifiedAccess>]
module MetadataConsts =
    let LANGUAGEID_MASK = 0b00000000000000000000000011111111
    let TOKEN_TYPE_MASK = 0b00000000000000000000011100000000
    let FONT_STYLE_MASK = 0b00000000000000000011100000000000
    let FOREGROUND_MASK = 0b00000000011111111100000000000000
    let BACKGROUND_MASK = 0b11111111100000000000000000000000

    let LANGUAGEID_OFFSET = 0
    let TOKEN_TYPE_OFFSET = 8
    let FONT_STYLE_OFFSET = 11
    let FOREGROUND_OFFSET = 14
    let BACKGROUND_OFFSET = 23

type [<AllowNullLiteral>] ITokenizeLineResult2 =
    /// The tokens in binary format. Each token occupies two array indices. For token i:
    ///   - at offset 2*i => startIndex
    ///   - at offset 2*i + 1 => metadata
    abstract tokens: Uint32Array
    /// The `prevState` to be passed on to the next line tokenization.
    abstract ruleStack: StackElement

type [<AllowNullLiteral>] IToken =
    abstract startIndex: float with get, set
    abstract endIndex: float
    abstract scopes: ResizeArray<string>

/// **IMPORTANT** - Immutable!
type [<AllowNullLiteral>] StackElement =
    abstract _stackElementBrand: unit with get //, set
    abstract depth: float
    abstract clone: unit -> StackElement
    abstract equals: other: StackElement -> bool


let [<Import("*","fast-plist")>] ``fast-plist``: Fast_plist.IExports = jsNative

module Fast_plist =

    type [<AllowNullLiteral>] IExports =
        /// A very fast plist parser
        abstract parse: content: string -> IRawTheme
