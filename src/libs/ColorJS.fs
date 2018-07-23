// ts2fable 0.6.1
module rec ColorJS
open System
open Fable.Core
open Fable.Import.JS

[<Import("default", "colors/safe")>]
let safe : Color = jsNative

type [<AllowNullLiteral>] IExports =
    abstract enable: unit -> unit
    abstract disable: unit -> unit
    abstract setTheme: theme: obj option -> unit
    abstract enabled: bool
    abstract strip: Color
    abstract stripColors: Color
    abstract black: Color
    abstract red: Color
    abstract green: Color
    abstract yellow: Color
    abstract blue: Color
    abstract magenta: Color
    abstract cyan: Color
    abstract white: Color
    abstract gray: Color
    abstract grey: Color
    abstract bgBlack: Color
    abstract bgRed: Color
    abstract bgGreen: Color
    abstract bgYellow: Color
    abstract bgBlue: Color
    abstract bgMagenta: Color
    abstract bgCyan: Color
    abstract bgWhite: Color
    abstract reset: Color
    abstract bold: Color
    abstract dim: Color
    abstract italic: Color
    abstract underline: Color
    abstract inverse: Color
    abstract hidden: Color
    abstract strikethrough: Color
    abstract rainbow: Color
    abstract zebra: Color
    abstract america: Color
    abstract trap: Color
    abstract random: Color
    abstract zalgo: Color

type [<AllowNullLiteral>] Color =
    [<Emit "$0($1...)">] abstract Invoke: text: string -> string
    abstract strip: Color with get, set
    abstract stripColors: Color with get, set
    abstract black: Color with get, set
    abstract red: Color with get, set
    abstract green: Color with get, set
    abstract yellow: Color with get, set
    abstract blue: Color with get, set
    abstract magenta: Color with get, set
    abstract cyan: Color with get, set
    abstract white: Color with get, set
    abstract gray: Color with get, set
    abstract grey: Color with get, set
    abstract bgBlack: Color with get, set
    abstract bgRed: Color with get, set
    abstract bgGreen: Color with get, set
    abstract bgYellow: Color with get, set
    abstract bgBlue: Color with get, set
    abstract bgMagenta: Color with get, set
    abstract bgCyan: Color with get, set
    abstract bgWhite: Color with get, set
    abstract reset: Color with get, set
    abstract bold: Color with get, set
    abstract dim: Color with get, set
    abstract italic: Color with get, set
    abstract underline: Color with get, set
    abstract inverse: Color with get, set
    abstract hidden: Color with get, set
    abstract strikethrough: Color with get, set
    abstract rainbow: Color with get, set
    abstract zebra: Color with get, set
    abstract america: Color with get, set
    abstract trap: Color with get, set
    abstract random: Color with get, set
    abstract zalgo: Color with get, set
