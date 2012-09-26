module TestRunner
    open System

    exception ExpectEx of string
    let test name f = 
        name,
        (fun () ->
            
            try
                f()
                None
            with exn ->
                Some exn)
    
    /// Test with expected exception testExn should return true wenn exception is expected
    let testExpectExn testExn name f = 
        (fun () ->
            name,
            try
                f()
                raise (ExpectEx "Expected exception!")
            with exn ->
                if testExn(exn) then
                    None
                else
                    Some exn)

    let writeConsole color f = 
        let old = System.Console.ForegroundColor
        try
            System.Console.ForegroundColor <- color
            f()
        finally
            System.Console.ForegroundColor <- old
    
    let printColor color (text:String) = 
        writeConsole color (fun () -> Console.WriteLine(text))

    let runtests allTests = 
        for t in allTests do
            let name, testFun = t
            printColor ConsoleColor.Green (sprintf "Test %s started" name)
            writeConsole ConsoleColor.Yellow (fun () ->
                match testFun() with
                | Some exn -> 
                    printColor ConsoleColor.Red (sprintf "Test %s failed with %O" name exn)
                | None ->
                    printColor ConsoleColor.Green (sprintf "Test %s succeeded!" name))


        