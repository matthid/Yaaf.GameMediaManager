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

    type WrapperTable<'T when 'T : not struct and 'T : equality and 'T : (new : unit -> 'T)> 
        internal (properties:System.Reflection.PropertyInfo seq) as x=   
        let table = new DataTable()  
        do 
            for p in properties do
                if p.DeclaringType <> typeof<'T> then
                    invalidOp 
                        (sprintf "propertyinfo must be from type 'T (%s)" typeof<'T>.FullName)
               
        let mutable initializer = (fun _ -> ())
        let properties = properties |> Seq.toList
        let mutable isInit = false
        let initColumns() =
            for name, t in
                    properties 
                        |> Seq.map (fun prop -> prop.Name, prop.PropertyType) do
                let newT = 
                    if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
                        Nullable.GetUnderlyingType t
                    else t
                table.Columns.Add(name, newT) |> ignore
            isInit <- true

        let rowReferences = new System.Collections.Generic.Dictionary<_,_>()
        let copyRowReferences = new System.Collections.Generic.Dictionary<_,_>()
        let itemReferences = new System.Collections.Generic.Dictionary<_,_>()
        let copyItemReferences = new System.Collections.Generic.Dictionary<_,_>()
        let copyLinqData = new System.Collections.Generic.HashSet<_>()
        let updateItem (data:'T) (row:DataRow) = 
            for prop, value in
                    properties 
                        |> Seq.map (fun prop -> prop, row.[prop.Name]) do
                let oldValue = Reflection.getPropertyValue prop data 
                let newValue =
                    match value with
                    | :? System.DBNull -> null
                    | _ -> value
                if (oldValue <> newValue) then
                    Reflection.setProperty prop data newValue

        let updateRow (data:'T) (row:DataRow) =
            for name, value in
                    properties 
                        |> Seq.map (fun prop -> prop.Name, data |> Reflection.getPropertyValue prop) do
                let newValue = 
                    if obj.ReferenceEquals(value, null) then System.DBNull.Value:>obj else value
                row.[name] <- newValue

        let addCopyForRow row = 
            let copy = new 'T()
            updateItem copy row
            copyLinqData.Add copy |> ignore
            copyRowReferences.Add(row, copy)
            copyItemReferences.Add(copy, row)  

        let addReferencesForNewItem row data =  
            rowReferences.Add(row, data)
            itemReferences.Add(data, row)  
            addCopyForRow row
        
        let changes = new System.Collections.Generic.List<_>()
        let inserts = new System.Collections.Generic.List<_>()
        let deletions = new System.Collections.Generic.List<_>()
        let mutable isListening = false
        let addData data = 
            let newRow = table.NewRow()
            updateRow data newRow
            //if not isListening then
            addReferencesForNewItem newRow data
//            else // force reference to be this
//                let oldData = rowReferences.[newRow]
//                rowReferences.[newRow] <- data
//                itemReferences.Remove oldData |> ignore
//                itemReferences.Add (data, newRow)
//                updateItem (copyRowReferences.Item newRow) newRow
            table.Rows.Add(newRow)


        let addAllData data = 
            for d in data do
                addData d

        let handleChanged row = 
            if not <| changes.Contains row then
                changes.Add row
                
            let copyItem = copyRowReferences.Item row
            updateItem copyItem row

        let handleDeletion row = 
            if inserts.Contains row then
                inserts.Remove row |> ignore
            else
                deletions.Add row
            copyLinqData.Remove (copyRowReferences.Item row) |> ignore
        let handleInsertion row = 
            if deletions.Contains row then
                deletions.Remove row |> ignore
                copyLinqData.Add (copyRowReferences.Item row) |> ignore
            else
                inserts.Add row
                if not <| rowReferences.ContainsKey row then
                    let newItem = new 'T()
                    initializer newItem
                    addReferencesForNewItem row newItem

        let handleNewRow row = 
            let newItem = new 'T()
            initializer newItem
            updateRow newItem row

        let startListen () =
            table.RowChanged
                |> Event.add 
                    (fun e ->
                        match e.Action with
                        | DataRowAction.Commit -> ()
                        | DataRowAction.Add -> handleInsertion e.Row
                        | _ -> handleChanged e.Row)
            table.RowDeleted |> Event.add (fun e -> handleDeletion e.Row)
            table.TableNewRow|> Event.add (fun e -> handleNewRow e.Row)
            isListening <- true
        let getInfo = (fun r -> rowReferences.Item r, r)
        
        let discardChangeData() = 
            deletions.Clear()
            changes.Clear()
            inserts.Clear()

        let updateTable (table:System.Data.Linq.Table<'T>) = 
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
            table.DeleteAllOnSubmit(deletions |> Seq.map getInfo |> Seq.map convert)
            changes |> updateAndConvert |> ignore
            discardChangeData()
        
        let updateFromChangedData copied = 
            let row = copyItemReferences.Item copied
            updateRow copied row 
            
        let updateItem originalItem newItem = 
            let row = itemReferences.Item originalItem
            updateRow newItem row

        let importChanges (otherTable:WrapperTable<'T>) = 
            let mapToRow = Seq.map 

            for item, copyItem in otherTable.Inserts |> Seq.map (fun (item, otherRow) -> item, otherTable.GetCopyItem otherRow) do
                addData item
                updateItem item copyItem

            for currentRow in otherTable.Deletions |> Seq.map (fun (item, otherRow) -> itemReferences.Item item) do
                currentRow.Delete()

            for origItem, changedItem in otherTable.Updates |> Seq.map (fun (i,r) -> i, otherTable.GetCopyItem r) do
                updateItem origItem changedItem

            table.AcceptChanges()
            otherTable.DiscardChanges()


        let fromCopiedData copiedData = 
            let newTable = new WrapperTable<_>(properties)
            newTable.InitColumns()
            for copyData, origData in 
                copiedData 
                    |> Seq.map 
                        (fun copy -> 
                            copy, 
                            let row = copyItemReferences.Item copy
                            rowReferences.Item row) do
                newTable.Add origData
                newTable.UpdateItem(origData, copyData)

            newTable.StartListening()
            newTable
        let clone () = 
            fromCopiedData copyLinqData

        let checkInit() =     
            if not isInit then
                invalidOp "Please init first!"

        member internal x.InitColumns () = 
            initColumns()
        member internal x.StartListening () = 
            checkInit()
            startListen()
        member internal x.UpdateItem (orig, changed) = 
            updateItem orig changed
            table.AcceptChanges()

        member x.UpdateItem (changedCopy) = updateFromChangedData(changedCopy)
        member internal x.AddItems items = 
            checkInit()
            addAllData items
        member internal x.Add item = 
            checkInit()
            addData item
        member internal x.DiscardChanges() = 
            checkInit()
            discardChangeData()
        
        member x.UpdateTable table = updateTable table
        member x.GetCopyItem row = copyRowReferences.Item row
        member x.Clone () = 
            checkInit()
            clone()
        member x.ImportChanges o = importChanges o
        member x.SetInitializer o = initializer <- o
        member x.SetInitializer (o:Action<_>) = initializer <- o.Invoke
        member x.GetCopiedTable copyData = fromCopiedData copyData
        member x.SourceTable 
            with get() = 
                checkInit()
                table
        member x.CopyLinqData 
           
            with get() = 
                checkInit()
                copyLinqData :> seq<_>
        member internal x.Inserts 
            with get() = inserts |> Seq.map getInfo
        member internal x.Deletions 
            with get() = deletions |> Seq.map getInfo
        member internal x.Updates 
            with get() = changes |> Seq.map getInfo

    


    let getProps<'a> filter = 
        typeof<'a>    
            |> getProperties allFlags 
            |> Seq.filter filter

    let getPropsDelegate<'a> (filter:Func<_,_>) = 
        getProps<'a> filter.Invoke

    let updateTable (wrapper:WrapperTable<_>) table = 
        wrapper.UpdateTable table

    let getFilter propNames = 
        (fun (prop:Reflection.PropertyInfo) -> 
            propNames |> Array.exists (fun name -> name = prop.Name))
    
    let getFilterDelegate propNames = 
        new Func<_,_>(getFilter propNames)

    let getWrapper filter (data:'a seq) = 
        let props = getProps<'a> filter
        let wrapper = new WrapperTable<_>(props)
        wrapper.InitColumns()
        wrapper.AddItems(data)
        wrapper.StartListening()
        wrapper

    let getWrapperDelegate (filter:Func<_,_>) (data:'a seq) = 
        getWrapper filter.Invoke data