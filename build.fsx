#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"

#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.JavaScript

Target.create "Clean" (fun _ ->
    !! "src/bin"
    ++ "src/obj"
    ++ "output"
    |> Seq.iter Shell.cleanDir
)

Target.create "Install" (fun _ ->
    DotNet.restore
        (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
        "src/code_lightner.fsproj"
)

Target.create "YarnInstall" (fun _ ->
    Yarn.install id
)

Target.create "Watch" (fun _ ->
    let result =
        DotNet.exec
            (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
            "fable"
            "yarn-run fable-splitter --port free -- --config src/splitter.config.js -w"

    if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode
)

Target.create "Test" (fun _ ->
    ()
)

Target.create "Release.Npm" (fun _ ->
    Shell.cleanDir "release"

    let result =
        DotNet.exec
            (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
            "fable"
            "yarn-run fable-splitter --port free -- --config src/splitter.config.js -o release"

    if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode
)

Target.create "Release.Fable" (fun _ ->
    Yarn.exec
        "rollup output/App.js --file release/main.js --format cjs"
        (fun o ->
           { o with WorkingDirectory = __SOURCE_DIRECTORY__ }
        )
)

// Build order
"Clean"
    ==> "Install"
    ==> "YarnInstall"
    ==> "Release.Npm"

"Watch"
    <== [ "YarnInstall" ]

// start build
Target.runOrDefault "Test"
