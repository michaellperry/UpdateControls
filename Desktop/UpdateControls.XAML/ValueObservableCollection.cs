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
            ((ValueObservableCollection)obj)._valueSource.TriggerUpdate();
        }

        private ValueSource _valueSource;
        private ObservableCollection<object> _collection = new ObservableCollection<object>();

        public ValueObservableCollection(string path, FrameworkElement targetObject, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
        {
            _valueSource = new ValueSource(
                path,
                targetObject,
                mode,
                updateSourceTrigger,
                OnUpdateValue,
                () => _collection,
                () => DispatcherSynchronizationContext.Current.Post(_callback, this));
        }

        public object Value
        {
            get { return _valueSource.Value; }
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return _valueSource.ProvideValue(serviceProvider, this);
        }

        private void OnUpdateValue(object v)
        {
            // Create a list of new items.
            List<ObservableCollectionItem> items = new List<ObservableCollectionItem>();

            // Dump all previous items into a recycle bin.
            using (RecycleBin<ObservableCollectionItem> bin = new RecycleBin<ObservableCollectionItem>())
            {
                foreach (object oldItem in _collection)
                    bin.AddObject(new ObservableCollectionItem(_collection, oldItem, true));

                // Add new objects to the list.
                if (v != null)
                {
                    foreach (object obj in (IEnumerable)v)
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
}
