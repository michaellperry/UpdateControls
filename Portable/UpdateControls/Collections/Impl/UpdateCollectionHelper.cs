using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Collections.Impl
{
	/// <summary>Helper structure used by DependentDictionary and 
	/// IndependentDictionary to represent the "Keys" and "Values" members.</summary>
	/// <remarks>
	/// If you save a reference to the Keys or Values property of <see cref="IndependentDictionary{TKey,TValue}"/>,
	/// the independent sentry should be informed when that collection is accessed. 
	/// This helper class ensures that the sentry is notified.
	/// <para/>
	/// For <see cref="DependentDictionary{TKey,TValue}"/>, this class is even more 
	/// important. Whenever DependentDictionary is updated, a new dictionary is 
	/// created to hold the updated content, so the Keys and Values collections 
	/// change frequently. This wrapper ensure that you do not accidentally hold 
	/// a reference to an out-of-date version of the Keys or Values collection. 
	/// It also ensures that the dictionary is updated if necessary when it is 
	/// accessed through the Keys or Values collection.
	/// </remarks>
	public struct UpdateCollectionHelper<T> : ICollection<T>
	{
		readonly Func<ICollection<T>> _get;

		public UpdateCollectionHelper(Func<ICollection<T>> getCollection)
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
