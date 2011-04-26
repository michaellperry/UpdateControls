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
using System.Diagnostics;

namespace UpdateControls
{
    /// <summary>
	/// Base class for <see cref="Dynamic"/> and <see cref="Dependent"/> sentries.
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
	/// <remarks>
	/// This class is for internal use only.
	/// </remarks>
	public abstract class Precedent
	{
        private class DependentNode
        {
            public Dependent Dependent;
            public DependentNode Next;
        }

		private DependentNode _firstDependent = null;

        /// <summary>
        /// Method called when the first dependent references this field. This event only
        /// fires when HasDependents goes from false to true. If the field already
        /// has dependents, then this event does not fire.
        /// </summary>
        protected virtual void GainDependent()
        {
        }

        /// <summary>
        /// Method called when the last dependent goes out-of-date. This event
        /// only fires when HasDependents goes from true to false. If the field has
        /// other dependents, then this event does not fire. If the dependent is
        /// currently updating and it still depends upon this field, then the
        /// GainDependent event will be fired immediately.
        /// </summary>
        protected virtual void LoseDependent()
        {
        }

		/// <summary>
		/// Establishes a relationship between this precedent and the currently
		/// updating dependent.
		/// </summary>
		internal void RecordDependent()
		{
			lock ( this )
			{
				// Get the current dependent.
				Dependent update = Dependent.GetCurrentUpdate();
                if (update != null && !DependencyExists(update))
				{
                    // Establish a two-way link.
                    update.AddPrecedent(this);
					bool gain = false;
					if ( _firstDependent == null )
						gain = true;
                    _firstDependent = new DependentNode { Dependent = update, Next = _firstDependent };
					if ( gain )
						GainDependent();
				}
				else if ( _firstDependent == null )
				{
					// Though there is no lasting dependency, someone
					// has shown interest.
					GainDependent();
					LoseDependent();
				}
			}
		}

		/// <summary>
		/// Makes all direct and indirect dependents out of date.
		/// </summary>
		internal void MakeDependentsOutOfDate()
		{
			// When I make a dependent out-of-date, it will
			// call RemoveDependent, thereby removing it from
			// the list.
			lock (this)
			{
				while (_firstDependent != null)
				{
					Dependent dependent = _firstDependent.Dependent;
					if (dependent != null)
						dependent.MakeOutOfDate();
					else
						_firstDependent = _firstDependent.Next;
				}
			}
		}

		internal void RemoveDependent( Dependent dependent )
		{
			//int before = _dependents.Count;
            DependentNode prior = null;
            DependentNode current = _firstDependent;
            while (current != null)
            {
                if (current.Dependent == dependent)
                {
                    if (prior == null)
                        _firstDependent = current.Next;
                    else
                        prior.Next = current.Next;
                }
                current = current.Next;
            }
			//Debug.Assert( _dependents.Count < before, "Dependent was not found in the collection." );
			if ( _firstDependent == null )
				LoseDependent();
		}

        /// <summary>
        /// True if any other fields depend upon this one.
        /// </summary>
        /// <remarks>
        /// If any dependent field has used this independent field while updating,
        /// then HasDependents is true. When that dependent becomes out-of-date,
        /// however, it no longer depends upon this field.
        /// <para/>
        /// This property is useful for caching. When all dependents are up-to-date,
        /// check this property for cached fields. If it is false, then nothing
        /// depends upon the field, and it can be unloaded. Be careful not to
        /// unload the cache while dependents are still out-of-date, since
        /// those dependents may in fact need the field when they update.
        /// </remarks>
        public bool HasDependents
		{
			get
			{
				lock ( this )
				{
					return _firstDependent != null;
				}
			}
		}

        private bool DependencyExists(Dependent update)
        {
            for (DependentNode current = _firstDependent; current != null; current = current.Next)
                if (current.Dependent == update)
                    return true;
            return false;
        }
    }
}
