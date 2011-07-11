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
	public class IndependentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
		private Independent _indDictionary = new NamedIndependent(MemoizedTypeName<IndependentDictionary<TKey, TValue>>.GenericName());

		public void Add(TKey key, TValue value)
		{
			_indDictionary.OnSet();
			_dictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			_indDictionary.OnGet();
			return _dictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { _indDictionary.OnGet(); return _dictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			_indDictionary.OnSet();
			return _dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			_indDictionary.OnGet();
			return _dictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get { _indDictionary.OnGet(); return _dictionary.Values; }
		}

		public TValue this[TKey key]
		{
			get
			{
				_indDictionary.OnGet();
				return _dictionary[key];
			}
			set
			{
				_indDictionary.OnSet();
				_dictionary[key] = value;
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			_indDictionary.OnSet();
			_dictionary.Add(item);
		}

		public void Clear()
		{
			_indDictionary.OnSet();
			_dictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			_indDictionary.OnGet();
			return _dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_indDictionary.OnGet();
			_dictionary.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { _indDictionary.OnGet(); return _dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			_indDictionary.OnSet();
			return _dictionary.Remove(item);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			_indDictionary.OnGet();
			return _dictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			_indDictionary.OnGet();
			return ((System.Collections.IEnumerable)_dictionary).GetEnumerator();
		}
	}
}
