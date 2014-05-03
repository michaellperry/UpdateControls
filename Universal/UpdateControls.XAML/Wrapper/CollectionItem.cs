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

using System;
using System.Collections.ObjectModel;

namespace UpdateControls.XAML.Wrapper
{
    class CollectionItem : IDisposable
    {
        private ObservableCollection<object> _collection;
        private object _item;
        private bool _inCollection;

        public CollectionItem(ObservableCollection<object> collection, object item, bool inCollection)
        {
            _collection = collection;
            _item = item;
            _inCollection = inCollection;
        }

        public void Dispose()
        {
            if (_inCollection)
                _collection.Remove(_item);
        }

        public void EnsureInCollection(int index)
        {
            if (!_inCollection)
            {
                // Insert the item into the correct position.
                _collection.Insert(index, _item);
            }
            else if (!object.Equals(_collection[index], _item))
            {
                // Remove the item from the old position.
                _collection.Remove(_item);

                // Insert the item in the correct position.
                _collection.Insert(index, _item);
            }
        }

        public override int GetHashCode()
        {
            return _item.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (!(obj is CollectionItem))
                return false;
            CollectionItem that = (CollectionItem)obj;
            return Object.Equals(
                this._item,
                that._item);
        }
    }
}
