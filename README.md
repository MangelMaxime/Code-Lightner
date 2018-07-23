# Code Lightner

:warning: *Current version of `code-lightner` only support Node.js.Browser support will be added later.* :warning:

### JavaScript version

## Installing

`npm install code-lightner`

## Using

```js
const code = require('code-lightner');

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
        backgroundColor: "#282c34", // Optional, set the background color of the pre element
        textColor: "#bbbbbbff", // Optional, set the color of the text
        grammarFiles: [ // Required, list of the grammar file to load
            "../syntaxes/JavaScript.tmLanguage.json",
            "../syntaxes/SQL.plist",
            "../syntaxes/hello.json",
            "../syntaxes/fsharp.json"
        ],
        scopeName: "source.fsharp", // Required, name of the scope to use on the provided code
        themeFile: "../themes/OneDark-Pro.json" // Required, path of the theme file
    };

code.lighten(config, sourceCode)
    .then(function(html) {
        console.log(html);
    });
```

### Fable version

**Coming soon**