module DatabaseTests

open TestRunner
let tests = [


    yield! 
        [1..1000]
        |> Seq.map (fun i ->
            simpleTest
                (sprintf "Test_%d" i) 
                (fun o -> async {
                    o System.ConsoleColor.Yellow "Before"
                    //do! Async.Sleep (1000*(i%5))
                    let childTask childId = async {
                        if (i%11 = 1) then
                            failwith "child failed!"
                        o System.ConsoleColor.Yellow "Child_Before"
                        System.Threading.Thread.Sleep 50
                        } 
                    do!
                        Seq.init 10 (fun i -> childTask i)
                            |> Async.Parallel
                            |> Async.Ignore

                    o System.ConsoleColor.Yellow "After"
                    return ()}))
        
    ]





