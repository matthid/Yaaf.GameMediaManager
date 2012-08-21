// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
[<AutoOpen>]
module HelperFunctions

    module Seq =
        /// Returns the first n items of s. If there are fewer items then alls are returned.
        let tryTake (n : int) (s : _ seq) =
            s 
                |> Seq.mapi (fun i t -> i < n, t)
                |> Seq.takeWhile (fun (shouldTake, t) -> shouldTake)
                |> Seq.map (fun (shouldTake, t) -> t)
    
    let curry f a b = f (a,b)
    let uncurry f (a,b) = f a b

    let pathCombine l = List.fold (curry System.IO.Path.Combine) "" l