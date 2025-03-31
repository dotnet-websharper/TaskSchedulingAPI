# WebSharper Prioritized Task Scheduling API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Prioritized Task Scheduling API](https://developer.mozilla.org/en-US/docs/Web/API/Scheduler), enabling efficient scheduling of tasks based on priority levels in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Prioritized Task Scheduling API.

2. **Sample Project**:
   - Demonstrates how to use the Prioritized Task Scheduling API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/TaskSchedulingAPI/)

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.TaskScheduling
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/TaskSchedulingAPI.git
   cd TaskSchedulingAPI
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.TaskScheduling/WebSharper.TaskScheduling.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.TaskScheduling.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action.

## Example Usage

Below is an example of how to use the Prioritized Task Scheduling API in a WebSharper project:

```fsharp
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
                Console.Error("\u274C Prioritized Task Scheduling API is not supported in this browser.")
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

                Console.Log("\u2705 Tasks have been scheduled!")
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
```

This example demonstrates how to schedule and execute tasks with different priority levels.

## Important Considerations

- **Browser Compatibility**: The Prioritized Task Scheduling API is currently supported in limited browsers. Check the [MDN Prioritized Task Scheduling API](https://developer.mozilla.org/en-US/docs/Web/API/Scheduler) for the latest support information.
- **Performance Optimization**: Use this API to optimize performance and responsiveness in web applications by prioritizing user-visible tasks.
- **Graceful Fallback**: Ensure fallback mechanisms are in place if the API is not supported in a user's browser.
