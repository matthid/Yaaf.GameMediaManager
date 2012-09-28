module DatabaseTests

open TestRunner
let tests = [


    yield! 
        [1..100]
        |> Seq.map (fun i ->
            simpleTest
                (sprintf "Test_%d" i) 
                (fun o -> async {
                    o System.ConsoleColor.Yellow "Before"
                    do! Async.Sleep (1000*(i%5))
                    o System.ConsoleColor.Yellow "After"
                    return ()}))
        
    ]





