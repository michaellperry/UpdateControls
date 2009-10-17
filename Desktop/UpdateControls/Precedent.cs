/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
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
	internal class Precedent
	{
		private List<Dependent> _dependents = new List<Dependent>();

		public event Action GainDependent;
		public event Action LooseDependent;

		/// <summary>
		/// Creates a new precedent sentry.
		/// </summary>
		public Precedent()
		{
		}

		/// <summary>
		/// Establishes a relationship between this precedent and the currently
		/// updating dependent.
		/// </summary>
		public void RecordDependent()
		{
			lock ( _dependents )
			{
				// Get the current dependent.
				Dependent update = Dependent.GetCurrentUpdate();
				if ( update != null && update.AddPrecedent( this ) )
				{
					// Establish a two-way link.
					bool gain = false;
					if ( _dependents.Count == 0 && GainDependent != null )
						gain = true;
					_dependents.Add( update );
					if ( gain )
						GainDependent();
				}
				else if ( _dependents.Count == 0 )
				{
					// Though there is no lasting dependency, someone
					// has shown interest.
					if ( GainDependent != null )
						GainDependent();
					if ( LooseDependent != null )
						LooseDependent();
				}
			}
		}

		/// <summary>
		/// Makes all direct and indirect dependents out of date.
		/// </summary>
		public void MakeDependentsOutOfDate()
		{
			// When I make a dependent out-of-date, it will
			// call RemoveDependent, thereby removing it from
			// the list.
			lock (_dependents)
			{
				while (_dependents.Count > 0)
				{
					Dependent dependent = _dependents[0];
					if (dependent != null)
						dependent.MakeOutOfDate();
					else
						_dependents.RemoveAt(0);
				}
			}
		}

		public void RemoveDependent( Dependent dependent )
		{
			int before = _dependents.Count;
			_dependents.Remove(dependent);
			Debug.Assert( _dependents.Count < before, "Dependent was not found in the collection." );
			if ( _dependents.Count == 0 && LooseDependent != null )
				LooseDependent();
		}

		public bool HasDependents
		{
			get
			{
				lock ( _dependents )
				{
					return _dependents.Count > 0;
				}
			}
		}
	}
}
