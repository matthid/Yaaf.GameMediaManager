// Weitere Informationen zu F# unter "http://fsharp.net".

for i in [1..1] do
    TestRunner.startTests
        (Seq.empty
        // |> Seq.append DatabaseTests.tests
         |> Seq.append EslGrabberTests.tests
         )
        |> Async.Start

System.Console.ReadLine() |> ignore