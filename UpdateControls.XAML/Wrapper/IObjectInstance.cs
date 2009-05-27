/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Windows;

namespace UpdateControls.XAML.Wrapper
{
    public interface IObjectInstance
    {
        ClassInstance ClassInstance { get; }
        object WrappedObject { get; }
        ObjectProperty LookupProperty(ClassProperty classProperty);
        void FirePropertyChanged(string name);
    }
}
