// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
open System
open System.IO

let path = "C:\\test"
let ext = ".jpg"

let fileSize rnd = (1024*7)
let filenumber = 1000000
let outputdelay = 1000
let globalRnd = Random()
let written = ref 0
let task() = async {
    let rnd =
        lock globalRnd (fun () ->
                Random(globalRnd.Next())
            )
    let createdFile = ref false
    while not <| !createdFile do
        let newFileName = sprintf "%s%s" (Guid.NewGuid().ToString()) ext
        let newPath = Path.Combine (path, newFileName)
        if not <| File.Exists newPath then
            let buffer = Array.zeroCreate (fileSize(rnd))
            rnd.NextBytes buffer
            File.WriteAllBytes (newPath, buffer)
            createdFile := true
            let newWritten = System.Threading.Interlocked.Increment(written)
            if (newWritten % outputdelay = 0) then
                printfn "Finished %d items" newWritten}

let tasks = async {
    do!
        Array.init filenumber (fun _ -> task())
        |> Async.Parallel
        |> Async.Ignore
    printfn "All Finished" }

let rec deleteTask path = async {
    printfn "Start deletion"
    Directory.EnumerateFiles(path)
    |> Seq.iteri 
        (fun i file ->
            try
                File.Delete file
            with :? IOException as exn -> 
                printfn "%s, Error, %O" file exn
            if (i%outputdelay = 0) then
                printfn "Finished %d items" i)
    do!
        Directory.EnumerateDirectories(path)
        |> Seq.map (fun p -> deleteTask p)
        |> Async.Parallel
        |> Async.Ignore
    printfn "Finished deletion"
    }
let deleteTasks = deleteTask path
//tasks |> Async.Start;;
//deleteTask |> Async.Start;;
