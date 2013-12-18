using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Forms.Util
{
    public static class CollectionHelper
    {
        private class Placeholder<T> : IDisposable
        {
            private IList _collection;
            private T _item;
            private bool _inCollection;

            public Placeholder(IList collection, T item, bool inCollection)
            {
                _collection = collection;
                _item = item;
                _inCollection = inCollection;
            }

            public T Item
            {
                get { return _item; }
            }

            public void EnsureInCollectionAt(int index)
            {
                if (!_inCollection)
                {
                    _collection.Insert(index, _item);
                }
                else if (!object.Equals(_collection[index], _item))
                {
                    _collection.Remove(_item);
                    _collection.Insert(index, _item);
                }
            }

            public void Dispose()
            {
                if (_inCollection)
                    _collection.Remove(_item);
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                Placeholder<T> that = obj as Placeholder<T>;
                if (that == null)
                    return false;
                return Object.Equals(this._item, that._item);
            }

            public override int GetHashCode()
            {
                return _item == null ? 0 : _item.GetHashCode();
            }
        }

        public static IEnumerable<T> RecycleCollection<T>(IList collection, IEnumerable<T> source)
        {
            // Recycle the collection of items.
            List<Placeholder<T>> newItems = new List<Placeholder<T>>(collection.Count);
            using (var recycleBin = new RecycleBin<Placeholder<T>>())
            {
                foreach (T item in collection)
                    recycleBin.AddObject(new Placeholder<T>(collection, item, true));

                // Extract each item from the recycle bin.
                foreach (T item in source)
                {
                    newItems.Add(recycleBin.Extract(new Placeholder<T>(collection, item, false)));
                }
            }

            for (int index = 0; index < newItems.Count; index++)
                newItems[index].EnsureInCollectionAt(index);

            return newItems.Select(placeholder => placeholder.Item);
        }
    }
}
