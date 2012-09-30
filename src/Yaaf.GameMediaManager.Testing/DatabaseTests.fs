module DatabaseTests
open System
open TestRunner

let crashTest s = null

let tests = [

    yield! 
        [1..50]
        |> Seq.map (fun i ->
            simpleTest
                (sprintf "Test_%d" i) 
                (fun o -> async {
                    o System.ConsoleColor.Yellow "Before"
                    //do! Async.Sleep (1000*(i%5))
                    let childTask childId = async {
                            return!
                                ((crashTest "blub"):System.String)
                                .ToLowerInvariant()
                                |> Seq.map (fun c -> async { return c })
                                |> Async.Parallel
                        } 
                    do!
                        Seq.init 10 (fun i -> childTask i)
                            |> Async.Parallel
                            |> Async.Ignore

                    o System.ConsoleColor.Yellow "After"
                    return ()}))
        
    ]





