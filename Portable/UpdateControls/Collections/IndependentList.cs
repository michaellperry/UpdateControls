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

using System.Collections.Generic;

namespace UpdateControls.Collections
{
	public class IndependentList<T> : IList<T>
	{
        private IList<T> _list;
		private Independent _indList = new NamedIndependent(MemoizedTypeName<IndependentList<T>>.GenericName());

        public IndependentList()
        {
            _list = new List<T>();
        }

        public IndependentList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        public int IndexOf(T item)
		{
			_indList.OnGet();
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_indList.OnSet();
			_list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_indList.OnSet();
			_list.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				_indList.OnGet();
				return _list[index];
			}
			set
			{
				_indList.OnSet();
				_list[index] = value;
			}
		}

		public void Add(T item)
		{
			_indList.OnSet();
			_list.Add(item);
		}

		public void Clear()
		{
			_indList.OnSet();
			_list.Clear();
		}

		public bool Contains(T item)
		{
			_indList.OnGet();
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_indList.OnGet();
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { _indList.OnGet(); return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			_indList.OnSet();
			return _list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			_indList.OnGet();
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			_indList.OnGet();
			return ((System.Collections.IEnumerable)_list).GetEnumerator();
		}
	}
}
