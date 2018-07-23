module Helpers

    module Promise =
        open Fable.Import
        open Fable.PowerPack
        open VSCode.TextMate

        let inline fromThenable (thenable : Thenable<'T>) : JS.Promise<'T> = unbox<JS.Promise<'T>> thenable

        let inline toThenable (promise : JS.Promise<'T>) : Thenable<'T> = unbox<Thenable<'T>> promise

        let inline ignore promise = Promise.map ignore promise
