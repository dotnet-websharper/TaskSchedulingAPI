namespace WebSharper.TaskScheduling

open WebSharper
open WebSharper.JavaScript

[<JavaScript; AutoOpen>]
module Extensions =

    type Window with
        [<Inline "$this.scheduler">]
        member this.Scheduler with get(): Scheduler = X<Scheduler>

    type WorkerGlobalScope with
        [<Inline "$this.scheduler">]
        member this.Scheduler with get(): Scheduler = X<Scheduler>

    type Navigator with
        [<Inline "$this.scheduling">]
        member this.Scheduling with get(): Scheduling = X<Scheduling>
