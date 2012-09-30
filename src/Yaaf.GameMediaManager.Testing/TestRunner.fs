module TestRunner
    open System

    exception ExpectEx of string

    type TestOptions = {
        Writer : ConsoleColor -> string -> unit}
    type TestResults = {
        Time : TimeSpan
        Failure : exn option
        }
    type Test = {
        Name : string
        Finished : IEvent<TestResults>
        SetFinished : TestResults -> unit
        TestFunc : TestOptions -> Async<TestResults> }

    let createTest name f =  
        let ev = new Event<TestResults>()
        {
            Name = name 
            Finished = ev.Publish
            SetFinished = (fun res -> ev.Trigger res)
            TestFunc = 
                (fun options -> async {
                    let watch = System.Diagnostics.Stopwatch.StartNew()
                    try
                        do! f options
                        watch.Stop()
                        return { Failure = None; Time = watch.Elapsed }
                    with exn ->
                        watch.Stop()
                        return { Failure = Some exn; Time = watch.Elapsed }
                    })}

    let simpleTest name f = 
        createTest name (fun options -> f options.Writer)
    
    let mapResult mapping test = 
        { test with
            TestFunc = 
                (fun options -> async {
                    let! result = test.TestFunc options
                    return mapping result})}

    /// Test with expected exception testExn should return true wenn exception is expected
    let simpleTestExpectExn testExn name f =
        simpleTest name f
        |> mapResult (fun res ->
            match res.Failure with
            | Some (exn) ->
                if testExn(exn) then
                    { res with Failure = None }
                else
                    { res with Failure = Some exn } 
            | None ->  { res with Failure = Some (exn "Should not happen")})    
    
    let writeConsole color f = 
        let old = System.Console.ForegroundColor
        try
            System.Console.ForegroundColor <- color
            f()
        finally
            System.Console.ForegroundColor <- old
    
    let printColor color (text:String) = 
        writeConsole color (fun _ -> Console.WriteLine(text))


    type WriterMessage = 
        | NormalWrite of ConsoleColor * String
        | StartTask of AsyncReplyChannel<int> * String
        | WriteMessage of int * ConsoleColor * String
        | EndTask of int

    let writer = MailboxProcessor.Start (fun inbox -> 
        let currentTask = ref 0
        let newHandle (returnHandle:AsyncReplyChannel<int>) = 
            let handle = System.Threading.Interlocked.Increment currentTask
            returnHandle.Reply handle
            handle 
        let rec loop tasks = async {
            let! newTasks =
                match tasks with
                | (t, name) :: next ->
                    inbox.Scan
                        (fun msg -> 
                            match msg with
                            | EndTask (endTask) -> 
                                if t = endTask then
                                    Some (async { return next })
                                else None
                            | WriteMessage(writeTask, color, message) ->
                                if writeTask = t then 
                                    Some (async {
                                        printColor color (sprintf "Task %s: %s" name message)
                                        return tasks
                                    })
                                else None
                            | StartTask (returnHandle, name) -> 
                                Some (async { 
                                    let handle = newHandle returnHandle
                                    return (List.append tasks [handle, name]) })
                            | _ -> None)
                | [] ->
                    inbox.Scan     
                        (fun msg -> 
                            match msg with
                            | StartTask (returnHandle, name) -> 
                                Some (async { 
                                    let handle = newHandle returnHandle
                                    return [handle, name] })
                            | NormalWrite(color, message) ->
                                Some (async {
                                    printColor color message
                                    return []
                                })
                            | _ -> None)   

            return! loop newTasks 
        }
        loop [])

    let writerWrite color (text:String) = 
        writer.Post(NormalWrite(color, text))

    let createTestWriter name f = async {
        let! handle = writer.PostAndAsyncReply(fun reply -> StartTask(reply, name))
        try
            let writer color s = 
                writer.Post(WriteMessage(handle,color,s))
            return! f(writer)
        finally
            writer.Post (EndTask(handle))
        }

    let testRun t = async {
        let! results = createTestWriter t.Name (fun writer -> async {
            writer ConsoleColor.Green (sprintf "started")
            let! results = t.TestFunc { Writer = writer }
            match results.Failure with
            | Some exn -> 
                writer ConsoleColor.Red (sprintf "failed with %O" exn)
            | None ->
                writer ConsoleColor.Green (sprintf "succeeded!")
            return results}) 
        t.SetFinished results
        }

    let runtests allTests = 
        for t in allTests do testRun t |> Async.RunSynchronously

    let startParallelMailbox workerNum f = 
        MailboxProcessor.Start(fun inbox ->
            let workers = Array.init workerNum (fun _ -> MailboxProcessor.Start f)
            let rec loop currentNum = async {
                let! msg = inbox.Receive()
                workers.[currentNum].Post msg
                return! loop ((currentNum + 1) % workerNum)
            }
            loop 0 )

    let testRunner = 
        startParallelMailbox 10 (fun inbox ->
            let rec loop () = async {
                let! test = inbox.Receive()
                do! testRun test 
                return! loop()
            }
            loop ())

    let startTests tests = async {
        let! results =
            tests 
                |> Seq.map (fun t ->
                    let waiter = t.Finished |> Async.AwaitEvent
                    testRunner.Post t
                    waiter
                    )
                |> Async.Parallel
        let testTime = 
            results
                |> Seq.map (fun res -> res.Time)
                |> Seq.fold (fun state item -> state + item) TimeSpan.Zero
        let failed = 
            results
                |> Seq.map (fun res -> res.Failure) 
                |> Seq.filter (fun o -> o.IsSome)
                |> Seq.length
        let testCount = results.Length
        if failed > 0 then
            writerWrite ConsoleColor.DarkRed (sprintf "--- %d of %d TESTS FAILED (%A) ---" failed testCount testTime)
        else
            writerWrite ConsoleColor.DarkGray (sprintf "--- %d TESTS FINISHED SUCCESFULLY (%A) ---" testCount testTime)
        }

        
//           
//        let last = tests |> Seq.last
//        last.Finished
//            |> Event.add (fun t -> printColor ConsoleColor.Green "--- ALL TESTS FINISHED ---")
//        for t in tests do