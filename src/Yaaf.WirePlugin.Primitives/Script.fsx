// Diese Datei ist ein Skript, das mit F# interaktiv ausgeführt werden kann.  
// Es kann zur Erkundung und zum Testen des Bibliotheksprojekts verwendet werden.
// Skriptdateien gehören nicht zum Projektbuild.

#r "D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\lib\HtmlAgilityPack\HtmlAgilityPack.dll"
#r "FSharp.PowerPack.dll";;
#load "FSharpInterop.fs"
open Yaaf.WirePlugin
open Yaaf.WirePlugin.Primitives
type GrabberProcMsg =
  | StartGrabbing
  | StoppedGrabbing
  | WaitForFinish of AsyncReplyChannel<unit>
  | GetNumber of AsyncReplyChannel<int>

let proc = MailboxProcessor.Start(fun inbox ->
    let rec loop grabbingNum (waitingList:AsyncReplyChannel<unit> list)= async {
        let! msg = inbox.Receive()
        match msg with
        | GrabberProcMsg.StartGrabbing ->
            printfn "StartGrabbing Message arrived"
            do! loop (grabbingNum + 1) waitingList
        | GrabberProcMsg.StoppedGrabbing ->
            printfn "StoppedGrabbing Message arrived"
            let newWaitingList = 
                if grabbingNum = 1 then
                    waitingList
                        |> List.iter (fun w -> w.Reply())
                    []
                else waitingList
            do! loop (grabbingNum - 1) newWaitingList
        | GrabberProcMsg.WaitForFinish(replyChannel) ->
            printfn "WaitForFinish Message arrived"
            let newWaitingList =
                if grabbingNum = 0 then
                    printfn "grabbingNum = 0"
                    waitingList
                        |> List.iter (fun w -> w.Reply())
                    replyChannel.Reply()
                    //inbox.PostAndAsyncReply(fun a -> GetNumber(a)) |> ignore
                    //replyChannel.Reply()
                    printfn "replied"
                    []
                else replyChannel :: waitingList
            do! loop grabbingNum newWaitingList
        | GrabberProcMsg.GetNumber (replyChannel) ->
            printfn "GetNumber Message arrived"
            replyChannel.Reply grabbingNum
            do! loop grabbingNum waitingList
        }
    loop 0 [])
proc.Error |> Event.add (fun e -> printfn "Mailbox problem %O" e)
let printNum () = 
    let num = proc.PostAndReply(fun channel -> GrabberProcMsg.GetNumber(channel))
    printfn "Number: %d" num
    
proc.Post GrabberProcMsg.StartGrabbing

let finishTask = proc.PostAndAsyncReply(fun channel -> GrabberProcMsg.WaitForFinish(channel));
let task = Primitives.Task<_> finishTask :> ITask<_>
task.Error |> Event.add (fun e -> printf "taskerror %O" e)
task.Finished |> Event.add (fun e -> printf "taskfinished %O" e)
task.Update |> Event.add (fun e -> printf "taskupdate %O" e)
task.Start()

async {
    try
        try
            do! async {
                do! Async.Sleep(1000)
                invalidOp "test"
            }
        with exn ->
            printfn "erro %O" exn
    finally
        proc.Post GrabberProcMsg.StoppedGrabbing
    } |> Async.Start

benc 
    [ "http://www.esl.eu/de/css/ui/versus/match/2778059"; 
      "http://www.esl.eu/de/css/ui/versus/match/2777955"; 
      "http://www.esl.eu/de/ui/versus/match/2758882"; 
      "http://www.esl.eu/de/css/ui/versus/match/2758197"; 
      "http://www.esl.eu/de/css/ui/versus/match/2754367"] getMatchMembers;;