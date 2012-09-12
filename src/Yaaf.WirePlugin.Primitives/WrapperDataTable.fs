// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin.Primitives

module WrapperDataTable = 
    open System.Data
    open System
    open Yaaf.WirePlugin.Primitives
    open Yaaf.WirePlugin.Primitives.Reflection

    type WrapperTable<'T when 'T : not struct>(filterProp, createFun:unit->'T) = 
        let table = new DataTable()
        
        let properties = 
            typeof<'T>    
                |> getProperties allFlags 
                |> Seq.filter filterProp
        do
            for name, t in
                    properties 
                        |> Seq.map (fun prop -> prop.Name, prop.PropertyType) do
                 table.Columns.Add(name, t) |> ignore
        let tableReferences = new System.Collections.Generic.Dictionary<_,_>()

        let setRow (data:'T) (row:DataRow) =
            for name, value in
                    properties 
                        |> Seq.map (fun prop -> prop.Name, data |> Reflection.getPropertyValue prop) do
                row.[name] <- value
        let addData data = 
            let newRow = table.NewRow()
            setRow data newRow
            tableReferences.Add(newRow, data)
            table.Rows.Add(newRow)

        let addAllData data = 
            for d in data do
                addData d

        let changes = new System.Collections.Generic.List<_>()
        let inserts = new System.Collections.Generic.List<_>()
        let deletions = new System.Collections.Generic.List<_>()
        let handleChanged row = 
            if not <| changes.Contains row then
                changes.Add row
        let handleDeletion row = 
            if inserts.Contains row then
                inserts.Remove row |> ignore
            else
                deletions.Add row
        let handleInsertion row = 
            if deletions.Contains row then
                deletions.Remove row |> ignore
            else
                let newItem = createFun()
                setRow newItem row
                inserts.Add row
                tableReferences.Add(row, newItem)

        let startListen () =
            table.RowChanged |> Event.add (fun e -> handleChanged e.Row)
            table.RowDeleted |> Event.add (fun e -> handleDeletion e.Row)
            table.TableNewRow|> Event.add (fun e -> handleInsertion e.Row)
        let updateItem (data:'T) (row:DataRow) = 
            for prop, newValue in
                    properties 
                        |> Seq.map (fun prop -> prop, row.[prop.Name]) do
                Reflection.setProperty prop data newValue

        let updateTable (table:System.Data.Linq.Table<'T>) = 
            let getInfo =  (fun r -> tableReferences.Item r, r)
            let update = 
                (fun (item, r) -> 
                    updateItem item r
                    item, r)
            let convert = (fun (item, r) -> item)
            let updateAndConvert e = 
                e 
                |> Seq.map getInfo 
                |> Seq.map update
                |> Seq.map convert
            table.InsertAllOnSubmit(inserts |> updateAndConvert)
            table.DeleteAllOnSubmit(deletions |> updateAndConvert)
            changes |> updateAndConvert |> ignore
                    
        member x.UpdateTable table = 
            updateTable table

        member x.StartListening () = 
            startListen()

        member x.AddItems items = 
            addAllData items

    let getWrapper filter createFun data = 
        let wrapper = new WrapperTable<_>(filter, createFun)
        wrapper.AddItems(data)
        wrapper.StartListening()
        wrapper

    let updateTable (wrapper:WrapperTable<_>) table = 
        wrapper.UpdateTable table

    let getFilter propNames = 
        (fun (prop:Reflection.PropertyInfo) -> 
            propNames |> Array.exists (fun name -> name = prop.Name))
