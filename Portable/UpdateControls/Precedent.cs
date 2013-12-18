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
using System.Linq;

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
		internal class DependentNode
        {
            public WeakReference Dependent;
            public DependentNode Next;
        }

        internal DependentNode _firstDependent = null;

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
            // Get the current dependent.
            Dependent update = Dependent.GetCurrentUpdate();
            if (update != null && !Contains(update) && update.AddPrecedent(this))
            {
                if (Insert(update))
                    GainDependent();
            }
            else if (!Any())
            {
                // Though there is no lasting dependency, someone
                // has shown interest.
                GainDependent();
                LoseDependent();
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
            Dependent first;
            while ((first = First()) != null)
            {
                first.MakeOutOfDate();
            }
        }

        internal void RemoveDependent(Dependent dependent)
        {
            if (Delete(dependent))
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
            get { return Any(); }
		}

        private bool Insert(Dependent update)
        {
            lock (this)
            {
                bool first = _firstDependent == null;
                _firstDependent = new DependentNode { Dependent = new WeakReference(update), Next = _firstDependent };
                return first;
            }
        }

        private static int _referenceCount = 0;

        private bool Delete(Dependent dependent)
        {
            lock (this)
            {
                int count = 0;
                DependentNode prior = null;
                for (DependentNode current = _firstDependent; current != null; current = current.Next)
                {
                    object target = current.Dependent.Target;
                    if (target == null || target == dependent)
                    {
                        if (target == null)
                            System.Diagnostics.Debug.WriteLine(String.Format("Dead reference {0}", _referenceCount++));
                        if (target == dependent)
                            ++count;
                        if (prior == null)
                            _firstDependent = current.Next;
                        else
                            prior.Next = current.Next;
                    }
                    else
                        prior = current;
                }
				if (count != 1) Debug.Assert(false, String.Format("Expected 1 dependent, found {0}.", count));
                return _firstDependent == null;
            }
        }

        private bool Contains(Dependent update)
        {
            lock (this)
            {
                for (DependentNode current = _firstDependent; current != null; current = current.Next)
                    if (current.Dependent.Target == update)
                        return true;
                return false;
            }
        }

        private bool Any()
        {
            lock (this)
            {
                return _firstDependent != null;
            }
        }

        private Dependent First()
        {
            lock (this)
            {
                while (_firstDependent != null)
                {
                    Dependent dependent = (Dependent)_firstDependent.Dependent.Target;
                    if (dependent != null)
                        return dependent;
                    else
                        _firstDependent = _firstDependent.Next;
                }
                return null;
            }
        }

		public override string ToString()
		{
			return VisualizerName(true);
		}

		#region Debugger Visualization

		/// <summary>Gets or sets a flag that allows extra debug features.</summary>
		/// <remarks>
		/// This flag currently just controls automatic name detection for untitled
		/// NamedIndependents, and other precedents that were created without a name 
		/// by calling <see cref="Independent.New"/>() or <see cref="Dependent.New"/>(),
		/// including dependents created implicitly by <see cref="GuiUpdateHelper"/>.
		/// <para/>
		/// DebugMode should be enabled before creating any UpdateControls sentries,
		/// otherwise some of them may never get a name. For example, if 
		/// Indepedent.New() is called (without arguments) when DebugMode is false, 
		/// a "regular" <see cref="Independent"/> is created that is incapable of 
		/// having a name.
		/// <para/>
		/// DebugMode may slow down your program. In particular, if you use named 
		/// independents (or <see cref="Independent{T}"/>) but do not explicitly 
		/// specify a name, DebugMode will cause them to compute their names based 
		/// on a stack trace the first time OnGet() is called; this process is
		/// expensive if it is repeated for a large number of Independents.
		/// </remarks>
		public static bool DebugMode { get; set; }
		
		public virtual string VisualizerName(bool withValue)
		{
			return VisNameWithOptionalHash(GetType().Name, withValue);
		}
		protected string VisNameWithOptionalHash(string name, bool withHash)
		{
			if (withHash) {
				// Unless VisualizerName has been overridden, we have no idea what 
				// value is associated with the Precedent. Include an ID code so 
				// that the user has a chance to detect duplicates (that is, when
				// he sees two Independents with the same code, they are probably 
				// the same Independent.)
				return string.Format("{0} #{1:X5}", name, GetHashCode() & 0xFFFFF);
			} else
				return name;
		}

		protected class DependentVisualizer
		{
			Precedent _self;
			public DependentVisualizer(Precedent self) { _self = self; }
			public override string ToString() { return _self.VisualizerName(true); }

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public DependentVisualizer[] Items
			{
				get {
					var list = new List<DependentVisualizer>();
					lock (_self)
					{
						for (DependentNode current = _self._firstDependent; current != null; current = current.Next)
						{
							var dep = current.Dependent.Target as Dependent;
							if (dep != null)
								list.Add(new DependentVisualizer(dep));
						}

						list.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

						// Return as array so that the debugger doesn't offer a useless "Raw View"
						return list.ToArray();
					}
				}
			}
		}

		#endregion
	}
}
