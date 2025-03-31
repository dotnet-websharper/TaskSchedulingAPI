namespace WebSharper.TaskScheduling

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let TaskPriority =
        Pattern.EnumStrings "TaskPriority" [
            "user-blocking"
            "user-visible"
            "background"
        ]

    let TaskPriorityChangeEventInit = 
        Pattern.Config "TaskPriorityChangeEventInit" {
            Required = []
            Optional = [
                "previousPriority", TaskPriority.Type
            ]
        }

    let TaskSignal =
        Class "TaskSignal"

    let AnyInit = 
        Pattern.Config "AnyInit" {
            Required = []
            Optional = [
                "priority", TaskPriority.Type + TaskSignal
            ]
        }

    let TaskPriorityChangeEvent =
        Class "TaskPriorityChangeEvent"
        |=> Inherits T<Dom.Event>
        |+> Static [
            Constructor (T<string>?eventType * !?TaskPriorityChangeEventInit?options)

            "any" => T<Array>?signals * !?AnyInit?init ^-> TaskSignal
        ]
        |+> Instance [
            "previousPriority" =? TaskPriority
        ]

    TaskSignal
    |=> Inherits T<Dom.AbortSignal>
    |+> Instance [
        "priority" =? TaskPriority

        "onprioritychange" =@ TaskPriorityChangeEvent ^-> T<unit>
        |> WithSourceName "onPriorityChange"
    ]
    |> ignore

    let TaskOptions =
        Pattern.Config "TaskOptions" {
            Required = []
            Optional = [
                "priority", TaskPriority.Type
                "signal", TaskSignal + T<Dom.AbortSignal>
                "delay", T<int>
            ]
        }

    let TaskControllerInit = 
        Pattern.Config "TaskControllerInit" {
            Required = []
            Optional = [
                "priority", TaskPriority.Type
            ]
        }

    let TaskController =
        Class "TaskController"
        |+> Static [
            Constructor (!?TaskControllerInit?options)
        ]
        |+> Instance [
            "setPriority" => TaskPriority?priority ^-> T<unit>
        ]

    let Scheduler =
        Class "Scheduler"
        |+> Instance [
            "postTask" => (T<unit> ^-> T<unit>) * !?TaskOptions?options ^-> T<Promise<_>>[T<unit>]
            "yield" => T<unit> ^-> T<Promise<_>>[T<unit>]
        ]

    let SchedulingInputPendingOptions = 
        Pattern.Config "SchedulingInputPendingOptions" {
            Required = []
            Optional = [
                "includeContinuous", T<bool>
            ]
        }

    let Scheduling =
        Class "Scheduling"
        |+> Instance [
            "isInputPending" => T<unit> * !?SchedulingInputPendingOptions?options ^-> T<bool>
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.TaskScheduling" [                
                Scheduling
                SchedulingInputPendingOptions
                TaskControllerInit
                AnyInit
                TaskPriorityChangeEventInit
                TaskPriority
                TaskOptions
                TaskController
                TaskSignal
                TaskPriorityChangeEvent
                Scheduler
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
