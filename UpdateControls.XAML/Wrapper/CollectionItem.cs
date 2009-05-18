/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.ObjectModel;

namespace UpdateControls.XAML.Wrapper
{
    abstract class CollectionItem : IDisposable
    {
        private ObservableCollection<object> _collection;
        private bool _inCollection;

        public CollectionItem(ObservableCollection<object> collection, bool inCollection)
        {
            _collection = collection;
            _inCollection = inCollection;
        }

        protected abstract object Item { get; }

        public void Dispose()
        {
            if (_inCollection)
                _collection.Remove(Item);
        }

        public void EnsureInCollection(int index)
        {
            if (!_inCollection)
            {
                // Insert the item into the correct position.
                _collection.Insert(index, Item);
            }
            else if (!_collection[index].Equals(Item))
            {
                // Move the item to the correct position.
                _collection.Move(_collection.IndexOf(Item), index);
            }
        }
    }
}
