namespace WebSharper.TaskScheduling.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.TaskScheduling

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let runScheduledTasks () =
        promise {
            let scheduler = JS.Window.Scheduler

            if isNull scheduler then
                Console.Error("❌ Prioritized Task Scheduling API is not supported in this browser.")
            else
                // ✅ High priority task (executed first)
                do! scheduler.PostTask((fun _ -> 
                    Console.Log("🔥 High Priority Task: user-blocking")
                ), TaskOptions(Priority = TaskPriority.User_blocking))

                // ✅ Medium priority task (executed second)
                do! scheduler.PostTask((fun _ -> 
                    Console.Log("⚡ Medium Priority Task: user-visible")
                ), TaskOptions(Priority = TaskPriority.User_visible))

                // ✅ Low priority task (executed last)
                do! scheduler.PostTask((fun _ -> 
                    Console.Log("💤 Low Priority Task: background")
                ), TaskOptions(Priority = TaskPriority.Background))

                Console.Log("✅ Tasks have been scheduled!")
        }

    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            .RunTasks(fun _ ->
                async {
                    do! runScheduledTasks().AsAsync()
                }
                |> Async.StartImmediate
            )
            .Doc()
        |> Doc.RunById "main"
