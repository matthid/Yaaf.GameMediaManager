// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin.Primitives

module Reflection = 
    open System
    open System.Reflection
    let allFlags = 
        BindingFlags.NonPublic ||| BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.Static
    let publicFlags = 
        BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.Static
    let publicInstanceFlags = 
        BindingFlags.Instance ||| BindingFlags.Public
    
    let getType i = i.GetType()
    let getProperties flags (t:Type) = 
        t.GetProperties(flags)
    let getProperty (flags:BindingFlags) name (t:Type) = 
        t.GetProperty(name, flags)
    
    let setProperty (p:PropertyInfo) obj value  = 
        p.SetValue(obj, value, null)

    let getPropertyValue (p:PropertyInfo) obj  = 
        p.GetValue(obj, null)