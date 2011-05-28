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
using System.Collections.Generic;

namespace UpdateControls
{
	/// <summary>
	/// A collection that recycles objects during a dependent update.
	/// </summary>
	/// <remarks>
	/// Construct a recycle bin within a <see cref="Dependent"/>'s update
	/// function when that Dependent controls a collection. Fill the
	/// recycle bin with the contents of the dependent collection,
	/// then reconstruct the collection by extracting objects from the
	/// recycle bin. This prevents the unnecessary destruction and
	/// recreation of objects in the dependent collection.
	/// <para/>
	/// The recycle bin extracts objects based on a prototype. If
	/// the recycle bin contains an object matching the prototype
	/// according to <see cref="Object.GetHashCode"/> and
	/// <see cref="Object.Equals(object)"/>, then that matching object
	/// is extracted. If not, the prototype itself is used. It is
	/// imperitive that you properly implement GetHashCode and
	/// Equals in your recycled classes.
	/// <para/>
	/// Your implementation of GetHashCode and Equals must only consider
	/// fields that do not change. If a field can be changed, or is
	/// itself dependent, then it must not be used either as part of the
	/// hash code, or to determine equality. The best practice is to
	/// implement GetHashCode and Equals in terms of fields that are
	/// initialized by the constructor, and are thereafter immutable.
	/// <para/>
	/// The advantage of RecycleBin is not found in any time or memory savings.
	/// In fact, using RecycleBin in most cases adds a small amount of overhead.
	/// However, the advantage comes from preserving the dynamic and
	/// dependent state of the recycled objects. If your depenent collection
	/// contains only immutable objects (such as strings), there is no
	/// advantage to using a RecycleBin.
	/// </remarks>
	public class RecycleBin<T> : IDisposable
	{
        private class Recyclable<T> : IDisposable
        {
            private T _object;
            private IDisposable _disposable;

            public Recyclable(T o, IDisposable disposable)
            {
                _object = o;
                _disposable = disposable;
            }

            public T Object
            {
                get { return _object; }
            }

            public void Dispose()
            {
                if (_disposable != null)
                    _disposable.Dispose();
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                    return true;
                Recyclable<T> that = obj as Recyclable<T>;
                if (that == null)
                    return false;
                return System.Object.Equals(this._object, that._object);
            }

            public override int GetHashCode()
            {
                return _object == null ? 0 : _object.GetHashCode();
            }
        }
        private Dictionary<Recyclable<T>, Recyclable<T>> _recyclableObjects = new Dictionary<Recyclable<T>, Recyclable<T>>();
        private static Func<T, Recyclable<T>> _makeRecyclable = GenerateMakeRecyclable();

		/// <summary>
		/// Creates an empty recycle bin.
		/// </summary>
		/// <remarks>
		/// The recycle bin should be filled with objects from a dependent
		/// collection, and the collection should be emptied. Then it can be
		/// repopulaed by extraction from the bin.
		/// </remarks>
		public RecycleBin()
		{
		}

		/// <summary>
		/// Add an object to the recycle bin.
		/// </summary>
		/// <param name="recyclableObject">The object to put in the recycle bin.</param>
        public void AddObject(T recyclableObject)
        {
            Recyclable<T> recyclable = _makeRecyclable(recyclableObject);
            if (_recyclableObjects.ContainsKey(recyclable))
            {
                recyclable.Dispose();
            }
            else
            {
                _recyclableObjects.Add(recyclable, recyclable);
            }
        }

		/// <summary>
		/// If a matching object is in the recycle bin, remove and return it.
		/// Otherwise, return the prototype.
		/// </summary>
		/// <param name="prototype">An object equal to the one to be extracted.</param>
		/// <returns>The matching object that was added to the recycle bin, or
		/// the prototype if no such object is found.</returns>
		public T Extract( T prototype )
		{
            // See if a matching object is already in the recycle bin.
            Recyclable<T> match;
            Recyclable<T> recyclable = _makeRecyclable(prototype);
            if (_recyclableObjects.TryGetValue(recyclable, out match))
            {
                // Yes, so extract it.
                recyclable.Dispose();
                _recyclableObjects.Remove(recyclable);
                return match.Object;
            }
            else
            {
                // No, so use the prototype.
                return prototype;
            }
		}

		/// <summary>
		/// Disposes all objects remaining in the recycle bin.
		/// </summary>
		/// <remarks>
		/// Call this method at the end of the update function. Any objects
		/// that have not been recycled will be disposed, thus removing any
		/// dependencies they may have. This allows cached objects to be
		/// unloaded and garbage collected.
		/// </remarks>
		public void Dispose()
		{
			foreach ( Recyclable<T> obj in _recyclableObjects.Values )
			{
				obj.Dispose();
			}
		}

        private static Func<T, Recyclable<T>> GenerateMakeRecyclable()
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                return recyclableObject => new Recyclable<T>(recyclableObject, (IDisposable)recyclableObject);
            }
            else
                return recyclableObject => new Recyclable<T>(recyclableObject, null);
        }
    }
}
