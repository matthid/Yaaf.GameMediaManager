// Weitere Informationen zu F# unter "http://fsharp.net".

TestRunner.runtests
    (DatabaseTests.tests
     |> Seq.append EslGrabberTests.tests)

System.Console.ReadLine() |> ignore
    