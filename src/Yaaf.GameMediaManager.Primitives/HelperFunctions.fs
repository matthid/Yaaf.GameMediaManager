// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
[<AutoOpen>]
module HelperFunctions

    let flip f x y = f y x
        
    let curry f a b = f (a,b)
    let uncurry f (a,b) = f a b

    module Event =
        /// Executes f just after adding the event-handler
        let guard f (e:IEvent<'Del, 'Args>) = 
            let e = Event.map id e
            { new IEvent<'Args> with 
                member x.AddHandler(d) = e.AddHandler(d); f()
                member x.RemoveHandler(d) = e.RemoveHandler(d)
                member x.Subscribe(observer) = 
                  let rm = e.Subscribe(observer) in f(); rm }

    module Seq =
        /// Returns the first n items of s. If there are fewer items then alls are returned.
        let tryTake (n : int) (s : _ seq) =
            s 
                |> Seq.mapi (fun i t -> i < n, t)
                |> Seq.takeWhile (fun (shouldTake, t) -> shouldTake)
                |> Seq.map (fun (shouldTake, t) -> t)
        let tryHead (s : _ seq) = 
            let newSeq =
                s
                    |> tryTake 1
                    |> Seq.cache // Remove possible side effects
            if newSeq |> Seq.isEmpty then None else newSeq |> Seq.head |> Some
            
        let sortWith f e = 
            let e' = e |> Seq.toArray
            e' |> Array.sortInPlaceWith f
            e' |> Seq.readonly

        let sortWithBy (extractKey:'a -> 'TKey) f e = 
            e 
                |> sortWith
                    (fun item1 item2 -> f (extractKey item1) (extractKey item2))

    let pathCombine l = List.fold (curry System.IO.Path.Combine) "" l