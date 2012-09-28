// Weitere Informationen zu F# unter "http://fsharp.net".

TestRunner.startTests
    (DatabaseTests.tests
     //|> Seq.append EslGrabberTests.tests
     )

System.Console.ReadLine() |> ignore
    