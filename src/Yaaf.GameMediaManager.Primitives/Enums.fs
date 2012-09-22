// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.Utils
open System

/// Helper Type for Typesafe "Enum"-Class, 'a = enum type, 'b IL type of the enum (int for most enums)
type Enums<'a,'b when 'a : struct and 'a : comparison and 'a : enum<'b> and 'b : comparison> private() = 
    // Collect Some Info about the Enum
    static let t = typeof<'a>
    static let hasFlagsAttribute =
        t.GetCustomAttributes(typeof<FlagsAttribute>,true).GetLength(0) > 0
    static let all = Enum.GetValues(typeof<'a>) |> Seq.cast<'a> |> Seq.cache


    // Primitive Converters
    static let convertToB (a:'a) = 
        (LanguagePrimitives.EnumToValue a:'b)
    static let convertToA (b:'b) = 
        (LanguagePrimitives.EnumOfValue b:'a)

    static let convertToLong (a:'a) = Convert.ChangeType(a, typeof<int64>) :?> int64
    static let convertFromLong (l:int64) = 
        let b = Convert.ChangeType(l, typeof<'b>) :?> 'b
        convertToA b

    // Some Lookup maps
    static let allWithKey = all |> Seq.map (fun v -> Enum.GetName(t, v),v)
    static let insensitiveNames = allWithKey |> Seq.map (fun (k, v) -> k.ToLowerInvariant(),v) |> Map.ofSeq
    static let sensitiveNames = allWithKey |> Map.ofSeq
    static let values = all |> Seq.map (fun v -> convertToB v, v) |> Map.ofSeq
    static let names = all |> Seq.map (fun v -> v, v.ToString()) |> Map.ofSeq

    // Parsing and helper functions
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

    static let hasFlag (flag:'a) (v:'a) = 
        let flagValue = convertToLong flag
        let valuesInLong = convertToLong v
        if hasFlagsAttribute then
            flagValue &&& valuesInLong = flagValue
        else 
            flagValue = valuesInLong

    /// Checks if the given value is defined in the enum
    static member IsDefined v = names.ContainsKey v
    /// Checks if the given string is defined in the enum
    static member IsDefined s = sensitiveNames.ContainsKey s
    /// Gets all values from the enum
    static member GetValues () = all
    /// Gets all names from the enum
    static member GetNames () = names |> Seq.map (fun kv -> kv.Value)
    /// Get all flags that are set in the given value (NOTE: will return all enum values which are set for Bitwise-Enums for example One, Two, Three for 3)
    static member GetFlags (v:'a) = getFlags v
    /// Gets the name of the given value if it is defined
    static member GetName (v:'a) = getName v
    /// Gets the value of the given enum-value
    static member GetValue (v:'a) = convertToB v
    /// Converts a value to the enum value
    static member FromValue (v:'b) = convertToA v
    /// Gets a string representing the values (like .ToString() with ", " seperated)
    static member ToString (v:'a) = toString ", " v
    /// Gets a string representing the values with split seperated
    static member ToString (v:'a, split) = toString split v
    /// Checks if the given value has the given flag (if it is no bitflag enum we will check equality)
    static member HasFlag (v:'a, flag:'a) = hasFlag flag v
    /// Parses the given string to is value
    static member Parse s = (parse false s):'a
    /// Tries to parse the given string to its value 
    static member TryParse (s, ignoreCase,[<System.Runtime.InteropServices.Out>]r:'a byref) =
        let t = ref r
        let b = tryParse ignoreCase s t
        r <- !t
        b
        
    /// Tries to parse the given string to its value respects case
    static member TryParse (s, [<System.Runtime.InteropServices.Out>]r:'a byref) = 
        let t = ref r
        let b = tryParse false s t
        r <- !t
        b
