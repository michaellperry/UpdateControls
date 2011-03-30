using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UpdateControls.Forms.UnitTest
{
    public class NonClearableArrayList : IList
    {
        private ArrayList _list = new ArrayList();
        private int _itemsRemovedCount = 0;

        public int Add(object value)
        {
            return _list.Add(value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _list.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _list.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _list.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return _list.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public void Remove(object value)
        {
            _list.Remove(value);
            _itemsRemovedCount++;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            _itemsRemovedCount++;
        }

        public object this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            _list.CopyTo(array, index);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsSynchronized
        {
            get { return _list.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _list.SyncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int ItemsRemovedCount
        {
            get { return _itemsRemovedCount; }
        }
    }
}
