/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System.Collections.Generic;
using UpdateControls.XAML.Wrapper;
using System;
using System.Linq;

namespace UpdateControls.XAML
{
    public static class ForView
    {
        /// <summary>
        /// Wrap an object to be used as the DataContext of a view.
        /// All of the properties of the object are available for
        /// data binding with automatic updates.
        /// </summary>
        /// <param name="wrappedObject">The object to wrap for the view.</param>
        /// <typeparam name="TWrappedObjectType">!!!DO NOT SPECIFY!!!</typeparam>
        /// <returns>An object suitable for data binding.</returns>
        public static object Wrap<TWrappedObjectType>(TWrappedObjectType wrappedObject)
        {
            return typeof(ObjectInstance<>)
                .MakeGenericType(wrappedObject.GetType())
                .GetConstructors()
                .Single()
                .Invoke(new object[] { wrappedObject, new Dictionary<object, IObjectInstance>() });
        }

        /// <summary>
        /// Unwrap a DataContext to get back to the original object.
        /// </summary>
        /// <typeparam name="TWrappedObjectType">The type of the object that was wrapped.</typeparam>
        /// <param name="dataContext">The DataContext previously wrapped.</param>
        /// <returns>The object originally wrapped, or null.</returns>
        public static TWrappedObjectType Unwrap<TWrappedObjectType>(object dataContext)
            where TWrappedObjectType : class
        {
            IObjectInstance wrapper = dataContext as IObjectInstance;
            return
                wrapper == null
                    ? default(TWrappedObjectType)
                    : wrapper.WrappedObject as TWrappedObjectType;
        }
    }
}
