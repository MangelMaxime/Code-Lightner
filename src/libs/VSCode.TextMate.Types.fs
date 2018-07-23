// ts2fable 0.6.1
module rec VSCode.TextMate.Types

open Fable.Core
open Fable.Import.JS


type [<AllowNullLiteral>] ILocation =
    abstract filename: string
    abstract line: float
    abstract char: float

type [<AllowNullLiteral>] ILocatable =
    abstract ``$vscodeTextmateLocation``: ILocation option

type [<AllowNullLiteral>] IRawGrammar =
    inherit ILocatable
    abstract repository: IRawRepository with get, set
    abstract scopeName: string
    abstract patterns: ResizeArray<IRawRule>
    abstract injections: obj option
    abstract injectionSelector: string option
    abstract fileTypes: ResizeArray<string> option
    abstract name: string option
    abstract firstLineMatch: string option

type [<AllowNullLiteral>] IRawRepositoryMap =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: name: string -> IRawRule with get, set
    abstract ``$self``: IRawRule with get, set
    abstract ``$base``: IRawRule with get, set

type [<AllowNullLiteral>] IRawRepository =
    interface end

type [<AllowNullLiteral>] IRawRule =
    inherit ILocatable
    abstract id: float option with get, set
    abstract ``include``: string option
    abstract name: string option
    abstract contentName: string option
    abstract ``match``: string option
    abstract captures: IRawCaptures option
    abstract ``begin``: string option
    abstract beginCaptures: IRawCaptures option
    abstract ``end``: string option
    abstract endCaptures: IRawCaptures option
    abstract ``while``: string option
    abstract whileCaptures: IRawCaptures option
    abstract patterns: ResizeArray<IRawRule> option
    abstract repository: IRawRepository option
    abstract applyEndPatternLast: bool option

type [<AllowNullLiteral>] IRawCapturesMap =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: captureId: string -> IRawRule with get, set

type [<AllowNullLiteral>] IRawCaptures =
    interface end

type [<AllowNullLiteral>] IOnigLib =
    abstract createOnigScanner: sources: ResizeArray<string> -> OnigScanner
    abstract createOnigString: sources: string -> OnigString

type [<AllowNullLiteral>] IOnigCaptureIndex =
    abstract start: float with get, set
    abstract ``end``: float with get, set
    abstract length: float with get, set

type [<AllowNullLiteral>] IOnigMatch =
    abstract index: float with get, set
    abstract captureIndices: ResizeArray<IOnigCaptureIndex> with get, set
    abstract scanner: OnigScanner with get, set

type [<AllowNullLiteral>] OnigScanner =
    abstract findNextMatchSync: string: U2<string, OnigString> * startPosition: float -> IOnigMatch

type [<AllowNullLiteral>] OnigString =
    abstract content: string
    abstract dispose: (unit -> unit) option
