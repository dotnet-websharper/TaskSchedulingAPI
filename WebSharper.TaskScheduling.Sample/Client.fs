namespace WebSharper.TaskScheduling.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.TaskScheduling

[<JavaScript>]
module Client =
    // The template is loaded from the DOM, so editing index.html and refreshing
    // the browser is enough; a rebuild is only needed when holes change.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let status = Var.Create "Ready to schedule tasks."
    let log = Var.Create ([] : string list)

    let runScheduledTasks () =
        promise {
            let scheduler = JS.Window.Scheduler

            if isNull scheduler then
                status := "⚠️ Prioritized Task Scheduling API is not supported in this browser."
                Console.Error("Prioritized Task Scheduling API is not supported in this browser.")
            else
                status := "Scheduling tasks…"
                log := []

                do! scheduler.PostTask((fun _ ->
                    Console.Log("🔥 High Priority Task: user-blocking")
                    log := log.Value @ [ "🔥 High priority — user-blocking" ]
                ), TaskOptions(Priority = TaskPriority.User_blocking))

                do! scheduler.PostTask((fun _ ->
                    Console.Log("⚡ Medium Priority Task: user-visible")
                    log := log.Value @ [ "⚡ Medium priority — user-visible" ]
                ), TaskOptions(Priority = TaskPriority.User_visible))

                do! scheduler.PostTask((fun _ ->
                    Console.Log("💤 Low Priority Task: background")
                    log := log.Value @ [ "💤 Low priority — background" ]
                ), TaskOptions(Priority = TaskPriority.Background))

                status := "✅ All tasks completed."
                Console.Log("✅ Tasks have been scheduled!")
        }

    [<SPAEntryPoint>]
    let Main () =
        // Run the demo once on load so the page shows meaningful results,
        // then let the button replay it.
        async {
            do! runScheduledTasks().AsAsync()
        }
        |> Async.StartImmediate

        IndexTemplate()
            .RunTasks(fun _ ->
                async {
                    do! runScheduledTasks().AsAsync()
                }
                |> Async.StartImmediate
            )
            .status(status.View)
            .log(Doc.BindSeqCached (fun entry -> Doc.Element "li" [] [ Doc.TextNode entry ]) log.View)
            .Bind()
