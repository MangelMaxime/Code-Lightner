const code = require('./../output/Main');

const sourceCode =
    `
    module Test

    [<RequireQualifiedAccess>]
    type FontStyle =
        | NotSet = -1
        | None = 0
        | Italic = 1
        | Bold = 2
        | Underline = 4
    `

let config =
    {
        backgroundColor: "#282c34",
        textColor: "#bbbbbbff",
        grammarFiles: [
            "./syntaxes/JavaScript.tmLanguage.json",
            "./syntaxes/SQL.plist",
            "./syntaxes/hello.json",
            "./syntaxes/fsharp.json"
        ],
        scopeName: "source.fsharp",
        themeFile: "./themes/OneDark-Pro.json"
    }

code.lighten(config, sourceCode)
    .then(function(html) {
        console.log(html);
    });
