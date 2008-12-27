/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace UpdateControls.XAML
{
    public class ValueObservableCollection
    {
        private static SendOrPostCallback _callback = new SendOrPostCallback(TriggerUpdate);

        private static void TriggerUpdate(object obj)
        {
            ((ValueObservableCollection)obj)._depValue.OnGet();
        }

        private string _path;
        private object _dataContext;

        private FrameworkElement _targetObject;
        private DependencyProperty _targetProperty;
        private Binding _binding = new Binding();

        private ObservableCollection<object> _collection = new ObservableCollection<object>();
        private Independent _indDataContext = new Independent();
        private Dependent _depValue;

        private MethodInfo _getMethod;
        private Dependent _depMethodInfo;

        public ValueObservableCollection(string path, FrameworkElement targetObject, DependencyProperty targetProperty)
        {
            _path = path;
            _targetObject = targetObject;
            _targetProperty = targetProperty;

            _depMethodInfo = new Dependent(UpdateMethodInfo);
            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
        }

        public BindingMode Mode
        {
            set { _binding.Mode = value; }
        }

        public UpdateSourceTrigger UpdateSourceTrigger
        {
            set { _binding.UpdateSourceTrigger = value; }
        }

        public object Value
        {
            get { _depValue.OnGet(); return _collection; }
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            // Register for notification when the data context changes.
            _targetObject.DataContextChanged += DataContextChanged;
            _dataContext = _targetObject.DataContext;

            // Initialize the property value.
            _depValue.OnGet();

            // Bind to the value dependency property.
            _binding.Source = this;
            _binding.Path = new PropertyPath("Value");
            return _binding.ProvideValue(serviceProvider);
        }

        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _indDataContext.OnSet();
            _dataContext = _targetObject.DataContext;
        }

        private void UpdateMethodInfo()
        {
            _getMethod = null;

            _indDataContext.OnGet();
            if (_dataContext != null)
            {
                // Find the property identified by path.
                Type contextType = _dataContext.GetType();
                MemberInfo[] member = contextType.GetMember(_path);
                if (member != null && member.Length == 1)
                {
                    PropertyInfo propertyInfo = member[0] as PropertyInfo;
                    if (propertyInfo != null && propertyInfo.CanRead)
                    {
                        _getMethod = propertyInfo.GetGetMethod();
                    }
                }
            }
        }

        private void UpdateValue()
        {
            _depMethodInfo.OnGet();
            if (_getMethod != null)
            {
                IEnumerable value = (IEnumerable)_getMethod.Invoke(_dataContext, null);

                // Create a list of new items.
                List<ObservableCollectionItem> items = new List<ObservableCollectionItem>();

                // Dump all previous items into a recycle bin.
                using (RecycleBin<ObservableCollectionItem> bin = new RecycleBin<ObservableCollectionItem>())
                {
                    foreach (object oldItem in _collection)
                        bin.AddObject(new ObservableCollectionItem(_collection, oldItem, true));

                    // Add new objects to the list.
                    if (value != null)
                    {
                        foreach (object obj in value)
                        {
                            items.Add(bin.Extract(new ObservableCollectionItem(_collection, obj, false)));
                        }
                    }

                    // All deleted items are removed from the collection at this point.
                }

                // Ensure that all items are added to the list.
                int index = 0;
                foreach (ObservableCollectionItem item in items)
                {
                    item.EnsureInCollection(index);
                    ++index;
                }
            }
        }

        private void ValueInvalidated()
        {
            DispatcherSynchronizationContext.Current.Post(_callback, this);
        }
    }
}
