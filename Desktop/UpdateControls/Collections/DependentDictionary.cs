using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Collections
{
	using UpdateControls.Collections.Impl;

	/// <summary>A dictionary tied to a dependent sentry.</summary>
	/// <remarks>
	/// To use DependentDictionary, you must pass a method to the constructor whose 
	/// job is to choose the contents of the dictionary (either as a list of key-
	/// value pairs, or as an object that implements <see cref="IDictionary{TKey,TValue}"/>).
	/// </remarks>
	public class DependentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> _computeCollection;
		private IDictionary<TKey, TValue> _dictionary;
		private Dependent _dependentSentry;
		private bool _recycleValues;

		/// <summary>Initializes DependentDictionary.</summary>
		/// <param name="updateCollection">A method that is called to choose the 
		/// contents of the dictionary.</param>
		/// <remarks>
		/// The update method will be called automatically when someone accesses the 
		/// dictionary, and either (1) it is being accessed for the first time, or
		/// (2) one of the precedents (Dependent and Independent sentries) that were 
		/// accessed by updateCollection() has changed since the last time it was
		/// called.
		/// <para/>
		/// DependentDictionary assumes that the "keys" are stateless objects that
		/// do not require recycling, but that values do require recycling. If the
		/// values are stateless, you will get better performance if you disable 
		/// recycling by adding a "false" parameter to the constructor, especially 
		/// if 'updateCollection' returns a dictionary directly. However, if the 
		/// values are viewmodels that contain state (such as an "is selected" 
		/// flag), and 'updateCollection' itself doesn't preserve this state, then 
		/// you should use recycling (which is the default) so that the extra state 
		/// information is not lost during updates.
		/// </remarks>
		public DependentDictionary(Func<IEnumerable<KeyValuePair<TKey, TValue>>> updateCollection) : this(updateCollection, true) { }
		public DependentDictionary(Func<IEnumerable<KeyValuePair<TKey, TValue>>> updateCollection, bool recycleValues)
		{
			_computeCollection = updateCollection;
			_dependentSentry = new NamedDependent(MemoizedTypeName<DependentDictionary<TKey, TValue>>.GenericName(), Update);
			_recycleValues = recycleValues;
		}

		void Update()
		{
			RecycleBin<TValue> vBin = null;
			try {
				if (_recycleValues && _dictionary != null)
				{
					vBin = new RecycleBin<TValue>();
					foreach (TValue item in _dictionary.Values)
						vBin.AddObject(item);
				}

				var result = _computeCollection();
				if (vBin != null || (_dictionary = (result as IDictionary<TKey, TValue>)) == null)
				{
					var dict = new Dictionary<TKey, TValue>();
					if (result != null)
					{
						if (vBin != null)
							foreach (var kvp in result)
								dict[kvp.Key] = vBin.Extract(kvp.Value);
						else
							foreach (var kvp in result)
								dict[kvp.Key] = kvp.Value;
					}
					_dictionary = dict;
				}
			}
			finally
			{
				if (vBin != null) vBin.Dispose();
			}
		}

		#region IDictionary<TKey,TValue> Members

		public void Add(TKey key, TValue value)
		{
			throw new NotSupportedException();
		}

		public bool ContainsKey(TKey key)
		{
			_dependentSentry.OnGet();
			return _dictionary.ContainsKey(key);
		}

		public DependentCollectionHelper<TKey> Keys
		{
			get { 
				return new DependentCollectionHelper<TKey>(() => {
					_dependentSentry.OnGet();
					return _dictionary.Keys;
				});
			}
		}
		ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return Keys; } }

		public bool Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			_dependentSentry.OnGet();
			return _dictionary.TryGetValue(key, out value);
		}

		public DependentCollectionHelper<TValue> Values
		{
			get { 
				return new DependentCollectionHelper<TValue>(() => {
					_dependentSentry.OnGet();
					return _dictionary.Values;
				});
			}
		}
		ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return Values; } }

		public TValue this[TKey key]
		{
			get {
				_dependentSentry.OnGet();
				return _dictionary[key];
			}
			set {
				throw new NotSupportedException();
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			_dependentSentry.OnGet();
			return _dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_dependentSentry.OnGet();
			_dictionary.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { _dependentSentry.OnGet(); return _dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			_dependentSentry.OnGet();
			return _dictionary.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public Dependent DependentSentry
		{
			get { return _dependentSentry; }
		}
	}

}

namespace UpdateControls.Collections.Impl
{
	/// <summary>Helper structure used by DependentDictionary to represent the 
	/// "Keys" and "Values" members.</summary>
	/// <remarks>
	/// Every time DependentDictionary is updated, a new dictionary is created to
	/// hold the updated content, so the Keys and Values collections change 
	/// frequently. This wrapper hides the changes to ensure that you do not 
	/// accidentally hold a reference to an out-of-date version of the Keys or
	/// Values collection. It also ensures that the dictionary is updated if 
	/// necessary when it is accessed through the Keys or Values collection.
	/// </remarks>
	public struct DependentCollectionHelper<T> : ICollection<T>
	{
		readonly Func<ICollection<T>> _get;

		public DependentCollectionHelper(Func<ICollection<T>> getCollection)
		{
			_get = getCollection;
		}

		public void Add(T item)
		{
			throw new NotSupportedException();
		}
		public void Clear()
		{
			throw new NotSupportedException();
		}
		public bool Contains(T item)
		{
			return _get().Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			_get().CopyTo(array, arrayIndex);
		}
		public int Count
		{
			get { return _get().Count; }
		}
		public bool IsReadOnly
		{
			get { return true; }
		}
		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _get().GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}