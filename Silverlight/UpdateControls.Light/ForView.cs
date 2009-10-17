/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System.Collections.Generic;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls
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
            return
                wrappedObject == null
                    ? null
                    : new ObjectInstance<TWrappedObjectType>(wrappedObject, new Dictionary<object, IObjectInstance>());
        }

        /// <summary>
        /// Unwrap a DataContext to get back to the original object.
        /// </summary>
        /// <typeparam name="TWrappedObjectType">The type of the object that was wrapped.</typeparam>
        /// <param name="dataContext">The DataContext previously wrapped.</param>
        /// <returns>The object originally wrapped, or null.</returns>
        public static TWrappedObjectType Unwrap<TWrappedObjectType>(object dataContext)
        {
            ObjectInstance<TWrappedObjectType> wrapper = dataContext as ObjectInstance<TWrappedObjectType>;
            return
                wrapper == null
                    ? default(TWrappedObjectType)
                    : (TWrappedObjectType)wrapper.WrappedObject;
        }
    }
}
