using System;
using System.Collections;

namespace UpdateControls.Forms.Util
{
	/// <summary>
	/// A delegate for mapping source to target objects.
	/// </summary>
	public delegate object MapDelegate( object source );

	/// <summary>
	/// A decorator for making lists read-only and applying maps.
	/// </summary>
	public class ReadOnlyListDecorator : IList
	{
		private IList _innerList;
		private MapDelegate _mapDelegate;

		private class Enumerator : IEnumerator
		{
			private IEnumerator _innerEnumerator;
			private ReadOnlyListDecorator _decorator;

			public Enumerator( ReadOnlyListDecorator decorator )
			{
				_innerEnumerator = decorator._innerList.GetEnumerator();
				_decorator = decorator;
			}

			#region IEnumerator Members

			public void Reset()
			{
				_innerEnumerator.Reset();
			}

			public object Current
			{
				get
				{
					return _decorator.Map( _innerEnumerator.Current );
				}
			}

			public bool MoveNext()
			{
				return _innerEnumerator.MoveNext();
			}

			#endregion
		}

		public ReadOnlyListDecorator( IList innerList ) :
			this( innerList, null )
		{
		}

		public ReadOnlyListDecorator( IList innerList, MapDelegate mapDelegate )
		{
			_innerList = innerList;
			_mapDelegate = mapDelegate;
		}

		private object Map( object source )
		{
			if ( _mapDelegate == null )
				return source;
			else
				return _mapDelegate( source );
		}

		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public object this[int index]
		{
			get
			{
				return Map( _innerList[ index ] );
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			foreach ( object item in _innerList )
				if ( Map( item ) == value )
					return true;
			return false;
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			for ( int index = 0; index < _innerList.Count; ++index )
				if ( Map( _innerList[index] ) == value )
					return index;
			return -1;
		}

		public int Add(object value)
		{
			throw new NotSupportedException();
		}

		public bool IsFixedSize
		{
			get
			{
				return _innerList.IsFixedSize;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return _innerList.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public object SyncRoot
		{
			get
			{
				return _innerList.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator( this );
		}

		#endregion
	}
}
