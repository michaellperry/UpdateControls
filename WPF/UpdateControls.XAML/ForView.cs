/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using UpdateControls.XAML.Wrapper;
using System.Windows.Threading;
using System;

namespace UpdateControls.XAML
{
    public static class ForView
    {
        private static Dispatcher _mainDispatcher;

        public static void Initialize()
        {
            // Ensure that the UpdateScheduler has the ability to run delegates
            // on the UI thread.
            if (_mainDispatcher == null)
            {
                _mainDispatcher = Dispatcher.CurrentDispatcher;
            }
            UpdateScheduler.Initialize(RunOnUIThread);
        }

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
            Initialize();
            if (wrappedObject == null)
                return null;
            return typeof(ObjectInstance<>)
				.MakeGenericType(wrappedObject.GetType())
				.GetConstructor(new Type[] { typeof(object), typeof(Dispatcher) })
				.Invoke(new object[] { wrappedObject, Dispatcher.CurrentDispatcher });
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

        private static void RunOnUIThread(Action action)
        {
            if (_mainDispatcher != null)
            {
                _mainDispatcher.BeginInvoke(action, DispatcherPriority.Background);
            }
        }
    }
}
