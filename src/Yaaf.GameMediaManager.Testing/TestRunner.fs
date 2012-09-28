module TestRunner
    open System

    exception ExpectEx of string

    type TestOptions = {
        Writer : ConsoleColor -> string -> unit}
    type TestResults = {
        Failure : exn option
        }
    type Test = {
        Name : string
        TestFunc : TestOptions -> Async<TestResults> }

    let createTest name f = {
        Name = name 
        TestFunc = 
            (fun options -> async {
                try
                    do! f options
                    return { Failure = None }
                with exn ->
                    return { Failure = Some exn }})}

    let simpleTest name f = 
        createTest name (fun options -> f options.Writer)
    
    let mapResult mapping test = {
        Name = test.Name 
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
        | StartTask of AsyncReplyChannel<int> * String
        | WriteMessage of int * ConsoleColor * String
        | EndTask of int


    let writer = MailboxProcessor.Start (fun inbox -> 
        let currentTask = ref 0
        let newHandle () = 
            System.Threading.Interlocked.Increment currentTask
        let rec loop tasks = async {
            let! ret, newTasks =
                match tasks with
                | (t, name) :: next ->
                    inbox.Scan
                        (fun msg -> 
                            match msg with
                            | EndTask (endTask) -> 
                                if t = endTask then
                                    Some (async { return None, next })
                                else None
                            | WriteMessage(writeTask, color, message) ->
                                if writeTask = t then 
                                    Some (async {
                                        printColor color (sprintf "Task %s: %s" name message)
                                        return None, tasks
                                    })
                                else None
                            | StartTask (returnHandle, name) -> 
                                let handle = newHandle ()
                                Some (async { return Some (returnHandle,handle), (List.append tasks [handle, name]) }))
                | [] ->
                    inbox.Scan     
                        (fun msg -> 
                            match msg with
                            | StartTask (returnHandle, name) -> 
                                let handle = newHandle ()
                                Some (async { return Some (returnHandle,handle), [handle, name] })
                            | _ -> None)   
            match ret with
            | Some (r,handle) -> r.Reply handle
            | None -> ()
            return! loop newTasks 
        }
        loop [])

    let createTestWriter name f = async {
        let! handle = writer.PostAndAsyncReply(fun reply -> StartTask(reply, name))
        try
            let writer color s = 
                writer.Post(WriteMessage(handle,color,s))
            do! f(writer)
        finally 
            writer.Post (EndTask(handle))
        }

    let testRun t = async {
        do! createTestWriter t.Name (fun writer -> async {
            writer ConsoleColor.Green (sprintf "started")
            let! results = t.TestFunc { Writer = writer }
            match results.Failure with
            | Some exn -> 
                writer ConsoleColor.Red (sprintf "failed with %O" exn)
            | None ->
                writer ConsoleColor.Green (sprintf "succeeded!")}) 
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

    let startTests test = 
        for t in test do testRunner.Post t