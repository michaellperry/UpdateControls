using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Collections
{
	class DependentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> _computeCollection;
		private IDictionary<TKey, TValue> _dictionary;
		private Dependent _dependentSentry;

		public DependentDictionary(Func<IEnumerable<KeyValuePair<TKey, TValue>>> computeCollection)
		{
			_computeCollection = computeCollection;

			_dependentSentry = new NamedDependent(MemoizedTypeName<DependentDictionary<TKey, TValue>>.GenericName(), Update);
		}

		void Update()
		{
				var result = _computeCollection();
				if ((_dictionary = (result as IDictionary<TKey, TValue>)) == null) {
					var dict = new Dictionary<TKey, TValue>();
					foreach (var kvp in result)
						dict[kvp.Key] = kvp.Value;
					_dictionary = dict;
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

		public ICollection<TKey> Keys
		{
			get { _dependentSentry.OnGet(); return _dictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			_dependentSentry.OnGet();
			_dictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get { _dependentSentry.OnGet(); return _dictionary.Values; }
		}

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
