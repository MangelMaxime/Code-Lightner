const code = require('./../output/Main');

const sourceCode =
    `
    open Thoth.Json.Encode

    let person =
        object
            [ "firstname", string "maxime"
              "surname", string "mangel"
              "age", int 25
              "address", object
                            [ "street", string "main street"
                              "city", string "Bordeaux" ]
            ]

    let compact = encode 0 person
    // {"firstname":"maxime","surname":"mangel","age":25,"address":{"street":"main street","city":"Bordeaux"}}

    let readable = encode 4 person
    // {
    //     "firstname": "maxime",
    //     "surname": "mangel",
    //     "age": 25,
    //     "address": {
    //         "street": "main street",
    //         "city": "Bordeaux"
    //     }
    // }
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
