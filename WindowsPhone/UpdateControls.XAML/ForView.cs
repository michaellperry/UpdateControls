/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System.Collections.Generic;
using UpdateControls.XAML.Wrapper;
using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows;

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
                _mainDispatcher = Deployment.Current.Dispatcher;
            }
            UpdateScheduler.Initialize(RunOnUIThread);
        }

        /// <summary>
        /// Wrap an object to be used as the DataContext of a view.
        /// All of the properties of the object are available for
        /// data binding with automatic updates.
        /// </summary>
        /// <param name="wrappedObject">The object to wrap for the view.</param>
        /// <returns>An object suitable for data binding.</returns>
        public static IObjectInstance Wrap(object wrappedObject)
        {
            Initialize();
            if (wrappedObject == null)
                return null;
            Tree tree = new Tree();
            IObjectInstance root = (IObjectInstance)typeof(ObjectInstance<>)
                .MakeGenericType(wrappedObject.GetType())
                .GetConstructors()
                .Single()
                .Invoke(new object[] { wrappedObject, tree });
            tree.SetRoot(root);
            return root;
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
                    ? null
                    : wrapper.WrappedObject as TWrappedObjectType;
        }

        public static void BindAppBarItem(object item, ICommand command)
        {
            var button = item as IApplicationBarMenuItem;
            if (button == null || command == null)
                return;

            button.Click += delegate
            {
                command.Execute(null);
            };

            command.CanExecuteChanged += delegate
            {
                button.IsEnabled = command.CanExecute(null);
            };
            button.IsEnabled = command.CanExecute(null);
        }

        private static void RunOnUIThread(Action action)
        {
            if (_mainDispatcher != null)
            {
                _mainDispatcher.BeginInvoke(action);
            }
        }
    }
}
