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
using UpdateControls.Fields;

namespace UpdateControls
{
	/// <summary>
	/// A sentry that controls a dependent field.
	/// <seealso cref="UpdateProcedure"/>
	/// <seealso cref="RecycleBin{T}"/>
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
	/// <remarks>
	/// <para>
	/// A dependent field is one whose value is determined by an update
	/// procedure. Use a Dependent sentry to control such a field.
	/// </para><para>
	/// Define a field of type Dependent in the same class as the dependent
	/// field, and initialize it with an update procedure, also defined
	/// within the class.
	/// </para><para>
	/// Calculate and set the dependent field within
	/// the update procedure. No other code should modify the dependent
	/// field.
	/// </para><para>
	/// Before each line of code that gets the dependent field, call
	/// the sentry's <see cref="Dependent.OnGet"/>. This will ensure
	/// that the field is up-to-date, and will record any dependencies
	/// upon the field.
	/// </para><para>
	/// If the dependent field is a collection, consider using a
	/// <see cref="RecycleBin{T}"/> to prevent complete destruction and
	/// recreation of the contents of the collection.
	/// </para>
	/// </remarks>
	/// <example>A class with a dependent field.
	/// <code language="C#">
	/// 	public class MyCalculatedObject
	/// 	{
	/// 		private MyDynamicObject _someOtherObject;
	/// 		private string _text;
	/// 		private Dependent _depText;
	/// 
	/// 		public MyCalculatedObject( MyDynamicObject someOtherObject )
	/// 		{
	/// 			_someOtherObject = someOtherObject;
	/// 			_depText = new Dependent( UpdateText );
	/// 		}
	/// 
	/// 		private void UpdateText()
	/// 		{
	/// 			_text = _someOtherObject.StringProperty;
	/// 		}
	/// 
	/// 		public string Text
	/// 		{
	/// 			get
	/// 			{
	/// 				_depText.OnGet();
	/// 				return _text;
	/// 			}
	/// 		}
	/// 	}
	/// </code>
    /// <code language="VB">
	/// Public Class MyCalculatedObject
	///     Private _someOtherObject As MyDynamicObject
	///     Private _text As String
	///     Private _depText As Dependent

	///     Public Sub New(ByVal someOtherObject As MyDynamicObject)
	///         _someOtherObject = someOtherObject
	///         _depText = New Dependent(New UpdateProcedure(UpdateText))
	///     End Sub

	///     Private Sub UpdateText()
	///         _text = _someOtherObject.StringProperty
	///     End Sub

	///     Public ReadOnly Property Text() As String
	///         Get
	///             _depText.OnGet()
	///             Return _text
	///         End Get
	///     End Property
	/// End Class
	/// </code>
	/// </example>
	public partial class Dependent : Precedent
	{
		public static Dependent New(Action update) { return DebugMode ? new NamedDependent(update) : new Dependent(update); }
		public static Dependent<T> New<T>(Func<T> update) { return new Dependent<T>(update); }
		public static NamedDependent New(string name, Action update) { return new NamedDependent(name, update); }
		public static Dependent<T> New<T>(string name, Func<T> update) { return new Dependent<T>(name, update); }

        private static ThreadLocal<Dependent> _currentUpdate = new ThreadLocal<Dependent>();

        internal static Dependent GetCurrentUpdate()
        {
            return _currentUpdate.Get();
        }

        /// <summary>
        /// Event fired when the dependent becomes out-of-date.
        /// <remarks>
        /// This event should not call <see cref="OnGet"/>. However, it should
        /// set up the conditions for OnGet to be called. For example, it could
        /// invalidate a region of the window so that a Paint method later calls
        /// OnGet, or it could signal a thread that will call OnGet.
        /// </remarks>
        /// </summary>
        public event Action Invalidated;

		protected internal Action _update;

		/// <summary>Gets the update method.</summary>
		/// <remarks>This property is used by GuiUpdateHelper.</remarks>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public Action UpdateMethod
		{
			get { return _update; }
		}

		private enum StatusType
		{
			OUT_OF_DATE,
			UP_TO_DATE,
			UPDATING,
			UPDATING_AND_OUT_OF_DATE,
            DISPOSED
		};
		private StatusType _status;
        private class PrecedentNode
        {
            public Precedent Precedent;
            public PrecedentNode Next;
        }
		private PrecedentNode _firstPrecedent = null;

		/// <summary>
		/// Creates a new dependent sentry with a given update procedure.
		/// <seealso cref="UpdateProcedure"/>
		/// </summary>
		/// <param name="update">The procedure that updates the value of the controled field.</param>
		/// <remarks>
		/// The update parameter is allowed to be null, so that derived classes
		/// can initialize properly. Due to a limitation of C#, an Update method 
		/// defined in a derived class can't be passed to the constructor of the 
		/// base class. Instead, update must be null and the _update member must 
		/// be set afterward.
		/// </remarks>
		public Dependent( Action update )
		{
			_update = update;
			_status = StatusType.OUT_OF_DATE;
		}

		/// <summary>
		/// Call this method before reading the value of a controlled field.
		/// </summary>
		/// <remarks>
		/// If the controlled field is out-of-date, this function calls the
		/// update procedure to bring it back up-to-date. If another dependent
		/// is currently updating, that dependent depends upon this one; when this
		/// dependent goes out-of-date again, that one does as well.
		/// </remarks>
		public void OnGet()
		{
			// Ensure that the attribute is up-to-date.
			if (MakeUpToDate())
			{
				// Establish dependency between the current update
				// and this attribute.
				RecordDependent();
			}
			else
			{
				// We're still not up-to-date (because of a concurrent change).
				// The current update should similarly not be up-to-date.
                Dependent currentUpdate = _currentUpdate.Get();
				if (currentUpdate != null)
					currentUpdate.MakeOutOfDate();
			}
		}

		/// <summary>
		/// Call this method to tear down dependencies prior to destroying
		/// the dependent.
		/// </summary>
		/// <remarks>
		/// While it is not absolutely necessary to call this method, doing
		/// so can help the garbage collector to reclaim the object. While
		/// the dependent is up-to-date, all of its precedents maintain
		/// pointers. Calling this method destroys those pointers so that
		/// the dependent can be removed from memory.
		/// </remarks>
		public void Dispose()
		{
			MakeOutOfDate();
            _status = StatusType.DISPOSED;
		}

        /// <summary>
        /// Read only property that is true when the dependent is up-to-date.
        /// </summary>
        public bool IsUpToDate
        {
			get
			{
				lock (this)
				{
					bool isUpToDate = _status == StatusType.UP_TO_DATE;
					return isUpToDate;
				}
			}
        }

        /// <summary>
        /// Read only property that is true when the dependent is not updating.
        /// </summary>
        public bool IsNotUpdating
        {
            get
            {
                lock (this)
                {
                    return
                        _status != StatusType.UPDATING &&
                        _status != StatusType.UPDATING_AND_OUT_OF_DATE;
                }
            }
        }

        /// <summary>
        /// Bring the dependent up-to-date, but don't take a dependency on it. This is
        /// useful for pre-loading properties of an object as it is created. It avoids
        /// the appearance of a list populated with empty objects while properties
        /// of that object are loaded.
        /// </summary>
        public void Touch()
        {
            MakeUpToDate();
        }

		internal void MakeOutOfDate()
		{
            bool wasUpToDate = false;
			lock ( this )
			{
				if ( _status == StatusType.UP_TO_DATE ||
					_status == StatusType.UPDATING )
				{
                    wasUpToDate = true;

                    // Tell all precedents to forget about me.
                    for (PrecedentNode current = _firstPrecedent; current != null; current = current.Next)
                        current.Precedent.RemoveDependent(this);

                    _firstPrecedent = null;

					if ( _status == StatusType.UP_TO_DATE )
						_status = StatusType.OUT_OF_DATE;
					else if ( _status == StatusType.UPDATING )
						_status = StatusType.UPDATING_AND_OUT_OF_DATE;

                    if (Invalidated != null)
                        Invalidated();
				}
			}

            if (wasUpToDate)
                // Make all indirect dependents out-of-date, too.
                MakeDependentsOutOfDate();
        }

		internal bool MakeUpToDate()
		{
			StatusType formerStatus;
			bool isUpToDate = true;

			lock ( this )
			{
				// Get the former status.
				formerStatus = _status;

				// Reserve the right to update.
				if ( _status == StatusType.OUT_OF_DATE )
					_status = StatusType.UPDATING;
			}

			if (formerStatus == StatusType.UPDATING ||
				formerStatus == StatusType.UPDATING_AND_OUT_OF_DATE)
			{
				// Report cycles.
				ReportCycles();
				//MLP: Don't throw, because this will mask any exception in an update which caused reentrance.
				//throw new InvalidOperationException( "Cycle discovered during update." );
			}
			else if (formerStatus == StatusType.OUT_OF_DATE)
			{
				// Push myself to the update stack.
				Dependent stack = _currentUpdate.Get();
                _currentUpdate.Set(this);

				// Update the attribute.
				try
				{
					_update();
				}
				finally
				{
					// Pop myself off the update stack.
					Dependent that = _currentUpdate.Get();
					Debug.Assert(that == this);
                    _currentUpdate.Set(stack);

					lock (this)
					{
						// Look for changes since the update began.
						if (_status == StatusType.UPDATING)
						{
							_status = StatusType.UP_TO_DATE;
						}
						else if (_status == StatusType.UPDATING_AND_OUT_OF_DATE)
						{
							_status = StatusType.OUT_OF_DATE;
							isUpToDate = false;
						}
						else
							Debug.Assert(false, "Unexpected state in MakeUpToDate");
					}
				}
			}

			return isUpToDate;
		}

		internal bool AddPrecedent( Precedent precedent )
		{
			lock ( this )
			{
				if ( _status == StatusType.UPDATING )
				{
                    _firstPrecedent = new PrecedentNode { Precedent = precedent, Next = _firstPrecedent };
                    return true;
				}
				else if ( _status != StatusType.UPDATING_AND_OUT_OF_DATE )
					Debug.Assert( false, "Unexpected state in AddPrecedent" );
                return false;
			}
		}

        static partial void ReportCycles();

		#region Debugger Visualization

		/// <summary>Intended for the debugger. Returns a tree of Dependents that 
		/// use this Dependent.</summary>
		/// <remarks>UsedBy is defined separately in Independent and Dependent so 
		/// that the user doesn't have to drill down to the final base class, 
		/// Precedent, in order to view this property.</remarks>
		protected DependentVisualizer UsedBy
		{
			get { return new DependentVisualizer(this); }
		}

		/// <summary>Intended for the debugger. Returns a tree of Precedents that 
		/// were accessed when this Dependent was last updated.</summary>
		protected PrecedentVisualizer Uses
		{
			get { return new PrecedentVisualizer(this); }
		}

		/// <summary>Intended for the debugger. Returns a tree of Precedents that 
		/// were accessed when this Dependent was last updated, collapsed so that
		/// all precedents that have the same name are shown as a single item.</summary>
		protected PrecedentSummarizer UsesSummary
		{
			get { return new PrecedentSummarizer(this); }
		}

		/// <summary>Helper class, intended to be viewed in the debugger, that 
		/// shows a list of Dependents and Independents that are used by this 
		/// Dependent.</summary>
		protected class PrecedentVisualizer
		{
			Dependent _self;
			public PrecedentVisualizer(Dependent self) { _self = self; }
			public override string ToString() { return _self.VisualizerName(true); }

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public object[] Items
			{
				get {
					var list = new List<object>();
					lock (_self)
					{
						for (PrecedentNode current = _self._firstPrecedent; current != null; current = current.Next)
						{
							var dep = current.Precedent as Dependent;
							var ind = current.Precedent as Independent;
							if (dep != null)
								list.Add(new PrecedentVisualizer(dep));
							else
								list.Add(new LeafVisualizer(ind));
						}
						
						list.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

						// Return as array so that the debugger doesn't offer a useless "Raw View"
						return list.ToArray();
					}
				}
			}
		}
		/// <summary>Helper class, used by <see cref="PrecedentVisualizer"/>, whose 
		/// ToString() method shows [I] plus the "extended name" of an Independent.</summary>
		private class LeafVisualizer
		{
			Independent _self;
			public LeafVisualizer(Independent self) { _self = self; }
			public override string ToString() { return _self.VisualizerName(true); }
		}

		/// <summary>Helper class, intended to be viewed in the debugger, that is 
		/// similar to PrecedentVisualizer except that it collapses all precedents 
		/// with the same name into a single entry.</summary>
		protected class PrecedentSummarizer
		{
			List<Precedent> _precedentsAtThisLevel;
			public PrecedentSummarizer(Precedent self)
			{
				_precedentsAtThisLevel = new List<Precedent>();
				_precedentsAtThisLevel.Add(self);
			}

			public override string ToString()
			{
				var list = _precedentsAtThisLevel;
				if (list.Count > 1)
					return string.Format("x{0} {1}", list.Count, list[0].VisualizerName(false));
				else if (list.Count == 1)
					return list[0].VisualizerName(true);
				else
					return "x0"; // should never happen
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public PrecedentSummarizer[] Items
			{
				get
				{
					var dict = new Dictionary<string, PrecedentSummarizer>();
					foreach (var item in _precedentsAtThisLevel)
					{
						if (item is Dependent) lock (item)
						{
							//if (_isDependentTree)
							//{
							//    for (DependentNode current = item._firstDependent; current != null; current = current.Next)
							//    {
							//        var dep = current.Dependent.Target as Dependent;
							//        if (dep != null)
							//        {
							//            PrecedentSummarizer child;
							//            if (dict.TryGetValue(dep.ToString(), out child))
							//                child._list.Add(dep);
							//            else
							//                dict[dep.ToString()] = new PrecedentSummarizer(dep, _isDependentTree);
							//        }
							//    }
							//}
							for (PrecedentNode current = (item as Dependent)._firstPrecedent; current != null; current = current.Next)
							{
								Precedent p = current.Precedent;
								string name = p.VisualizerName(false);
								PrecedentSummarizer child;
								if (dict.TryGetValue(name, out child))
									child._precedentsAtThisLevel.Add(current.Precedent);
								else
									dict[name] = new PrecedentSummarizer(p);
							}
						}
					}

					return dict.Values.ToArray();
				}
			}
		}

		#endregion
	}
}
