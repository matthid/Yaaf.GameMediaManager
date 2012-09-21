// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager
open System
[<Flags>]
type TestEnum = 
    | One = 1
    | Two = 2
    | Three = 3

type Enums<'a,'b when 'a : struct and 'a : comparison and 'a : enum<'b> and 'b : comparison> private() = 
    // and 'a : (static member (|||) : 'a * 'a -> 'a)
    static let t = typeof<'a>
    static let hasFlagsAttribute =
        t.GetCustomAttributes(typeof<FlagsAttribute>,true).GetLength(0) > 0
    static let all = Enum.GetValues(typeof<'a>) |> Seq.cast<'a> |> Seq.cache
    static let allWithKey = all |> Seq.map (fun v -> Enum.GetName(t, v),v)
    static let insensitiveNames = allWithKey |> Seq.map (fun (k, v) -> k.ToLowerInvariant(),v) |> Map.ofSeq
    static let sensitiveNames = allWithKey |> Map.ofSeq

    static let convertToB (a:'a) = 
        (LanguagePrimitives.EnumToValue a:'b)
        //Convert.ChangeType(a, typeof<'b>) :?> 'b
    static let convertToA (b:'b) = 
        LanguagePrimitives.EnumOfValue b

    static let convertToLong (a:'a) = Convert.ChangeType(a, typeof<int64>) :?> int64
    static let convertFromLong (l:int64) = 
        let b = Convert.ChangeType(l, typeof<'b>) :?> 'b
        convertToA b

    static let values = all |> Seq.map (fun v -> convertToB v, v) |> Map.ofSeq
    static let names = all |> Seq.map (fun v -> v, v.ToString()) |> Map.ofSeq
    static let myTryParseSingleValue ignoreCase s = 
        let searchDict = 
            if ignoreCase then insensitiveNames else sensitiveNames

        match searchDict.TryFind s with
        | Some s -> Some s
        | None ->
            // Check if we can parse from number
            match System.Int64.TryParse s with
            | true, l -> convertFromLong l |> Some
            | false, _ -> None
    
    static let myTryParse ignoreCase (s:String) = 
        let singleValues = 
            (if hasFlagsAttribute then s.Split [| '|'; ',' |] |> Seq.ofArray else seq { yield s })
                |> Seq.map (fun item -> item.Trim())
                |> Seq.map (myTryParseSingleValue ignoreCase)
                |> Seq.cache
        if singleValues |> Seq.exists (fun t -> t.IsNone) || singleValues |> Seq.isEmpty then
            None
        else
            let defaultB = Unchecked.defaultof<'b>
            singleValues 
                |> Seq.map (fun t -> t.Value)
                |> Seq.fold (fun state (item1:'a) -> state ||| (convertToLong item1)) (0L)
                |> convertFromLong
                |> Some

    static let parse ignoreCase s =
        match myTryParse ignoreCase s with
        | Some s -> s
        | None -> raise (new FormatException("Value is not one of the named constants defined for the enumeration"))
    static let tryParse ignoreCase s (ref:'a ref) =
        match myTryParse ignoreCase s with
        | Some s -> 
            ref := s
            true
        | None -> false
                    
    static let getName a = 
        match names.TryFind a with
        | Some s -> s
        | None -> null

    static let getFlags a =
        if hasFlagsAttribute then
            let value = convertToLong a
            let asLongs =
                all
                    |> Seq.map (fun v -> convertToLong v) 
                    |> Seq.filter (fun l -> value &&& l = l)
            if asLongs |> Seq.fold (fun state item -> state ||| item) 0L <> value then
                Seq.empty
            else
                asLongs
                    |> Seq.map (fun l -> convertFromLong l)
        else
            if names.ContainsKey a then
                seq { yield a }
            else
                Seq.empty

    static let toString split a = 
        getFlags a
            |> Seq.map (fun l -> names.Item l)
            |> String.concat split
//        if hasFlagsAttribute then
//            let value = convertToLong a
//            let asLongs =
//                all
//                    |> Seq.map (fun v -> convertToLong v) 
//                    |> Seq.filter (fun l -> value &&& l = l)
//            if asLongs |> Seq.fold (fun state item -> state ||| item) 0L <> value then
//                value.ToString()
//            else
//                asLongs
//                    |> Seq.map (fun l -> convertFromLong l)
//                    |> Seq.map (fun l -> names.Item l)
//                    |> String.concat split
//        else
//            getName a
//    

    static let hasFlag (flag:'a) (v:'a) = 
        let flagValue = convertToLong flag
        let valuesInLong = convertToLong v
        if hasFlagsAttribute then
            flagValue &&& valuesInLong = flagValue
        else 
            flagValue = valuesInLong

    static member IsDefined v = names.ContainsKey v
    static member IsDefined s = sensitiveNames.ContainsKey s
    static member GetValues () = all
    static member GetNames () = names |> Seq.map (fun kv -> kv.Value)
    static member GetFlags (v:'a) = getFlags v
    static member GetName (v:'a) = getName v
    static member ToString (v:'a) = toString ", " v
    static member ToString (v:'a, split) = toString split v
    static member HasFlag (v:'a, flag:'a) = hasFlag flag v
    static member Parse s = (parse false s):'a
    static member TryParse (s, ignoreCase,[<System.Runtime.InteropServices.Out>]r:'a byref) =
        let t = ref r
        let b = tryParse ignoreCase s t
        r <- !t
        b

    static member TryParse (s, [<System.Runtime.InteropServices.Out>]r:'a byref) = 
        let t = ref r
        let b = tryParse false s t
        r <- !t
        b
