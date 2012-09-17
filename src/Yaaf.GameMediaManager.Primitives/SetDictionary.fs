// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager.Primitives

type SetDictionary<'a, 'b when 'a : comparison>() = 
    let dict = new System.Collections.Generic.Dictionary<'a Set,'b>()

    let getSet list = 
        list |> Set.ofSeq
    let addItem list value = 
        let seq = getSet list
        dict.Add(seq, value)

    let setItem list value = 
        let seq = getSet list
        dict.[seq] <- value

    let containsKey list =
        let seq = getSet list 
        dict.ContainsKey seq

    let tryGetValue list = 
        let seq = getSet list
        let value = ref Unchecked.defaultof<'b>
        if dict.TryGetValue(seq, value) then
            Some !value
        else None


    let getItem list = 
        let seq = getSet list
        dict.Item seq

    member x.Item 
        with get(list) = 
            getItem list
        and set list data = 
            setItem list data
    member x.Add list value = addItem list value
    member x.ContailsKey list = containsKey list
    member x.TryGetValue list = tryGetValue list
    member x.RawDict 
        with get() = dict
