/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.ObjectModel;

namespace UpdateControls.XAML
{
    class ObservableCollectionItem : IDisposable
    {
        private ObservableCollection<object> _collection;
        private object _item;
        private bool _inCollection;

        public ObservableCollectionItem(ObservableCollection<object> collection, object item, bool inCollection)
        {
            _collection = collection;
            _item = item;
            _inCollection = inCollection;
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
            if (!(obj is ObservableCollectionItem))
                return false;
            ObservableCollectionItem that = (ObservableCollectionItem)obj;
            return this._item.Equals(that._item);
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
            else if (!_collection[index].Equals(_item))
            {
                // Move the item to the correct position.
                _collection.Move(_collection.IndexOf(_item), index);
            }
        }
    }
}
