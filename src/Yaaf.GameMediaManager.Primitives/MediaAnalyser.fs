﻿// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager
module MediaAnalyser = 
    open System.IO

    type MediaInfo = {
            Map : string;
        }

    /// entract data from .dem file
    let analyseDemo (path:string) = 
        use tr = new StreamReader(path)
        let buffer = Array.create 64 (char 0)
        let pos = ref 0

        let readWhile i = 
            while (not <| tr.EndOfStream && !pos < i) do
                tr.Read() |> ignore
                pos := !pos + 1
        let readString size = 
            tr.Read(buffer, 0, size) |> ignore
            pos := !pos + size
            (new System.String(buffer)).Trim(char 0)
        
        readWhile 16
        let server = readString 64
        readWhile 536
        let mapName = readString 64
        readWhile 796
        let gameName = readString 32
        { Map = mapName }

    let analysePicName (name:string) = 
        { Map = name.Substring(0, name.Length - 4) }

    let analyseSimpleName (name:string) = 
        let dupl = name.IndexOf '('
        let mapname = (
            if dupl = -1 
            then name
            else name.Substring(0, dupl)).Trim()
        { Map = mapname }

    let analyseMedia path = 
        let ext = Path.GetExtension(path)
        let name = Path.GetFileNameWithoutExtension(path)
        match ext with
        | ".dem" ->
            analyseDemo path
        | ".jpg" ->
            analysePicName name
        | ".SC2Replay" ->
            analyseSimpleName name
        | _ ->
            { Map = "_unknown_" }
            
