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
	public partial class Dependent
	{
        [ThreadStatic]
        private static Dependent _currentUpdate = null;

        internal static Dependent GetCurrentUpdate()
        {
            return _currentUpdate;
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

		internal Precedent _base = new Precedent();
		private Action _update;
		private enum StatusType
		{
			OUT_OF_DATE,
			UP_TO_DATE,
			UPDATING,
			UPDATING_AND_OUT_OF_DATE,
            DISPOSED
		};
		private StatusType _status;
		private List<Precedent> _precedents;

		/// <summary>
		/// Creates a new dependent sentry with a given update procedure.
		/// <seealso cref="UpdateProcedure"/>
		/// </summary>
		/// <param name="update">The procedure that updates the value of the controled field.</param>
		public Dependent( Action update )
		{
			_update = update;
			_status = StatusType.OUT_OF_DATE;
			_precedents = new List<Precedent>();
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
				_base.RecordDependent();
			}
			else
			{
				// We're still not up-to-date (because of a concurrent change).
				// The current update should similarly not be up-to-date.
                Dependent currentUpdate = _currentUpdate;
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
                lock ( _precedents )
                {
                    return _status == StatusType.UP_TO_DATE;
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
                lock (_precedents)
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
			lock ( _precedents )
			{
				if ( _status == StatusType.UP_TO_DATE ||
					_status == StatusType.UPDATING )
				{
					// Tell all precedents to forget about me.
					foreach ( Precedent precedent in _precedents )
					{
						if (precedent != null)
							precedent.RemoveDependent( this );
					}

					_precedents.Clear();

					// Make all indirect dependents out-of-date, too.
					_base.MakeDependentsOutOfDate();

					if ( _status == StatusType.UP_TO_DATE )
						_status = StatusType.OUT_OF_DATE;
					else if ( _status == StatusType.UPDATING )
						_status = StatusType.UPDATING_AND_OUT_OF_DATE;

                    if (Invalidated != null)
                        Invalidated();
				}
			}
		}

		internal bool MakeUpToDate()
		{
			StatusType formerStatus;
			bool isUpToDate = true;

			lock ( _precedents )
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
				Dependent stack = _currentUpdate;
                _currentUpdate = this;

				// Update the attribute.
				try
				{
					_update();
				}
				finally
				{
					// Pop myself off the update stack.
					Dependent that = _currentUpdate;
					Debug.Assert(that == this);
                    _currentUpdate = stack;

					lock (_precedents)
					{
						// Look for changes since the update began.
						if (_status == StatusType.UPDATING)
							_status = StatusType.UP_TO_DATE;
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
			lock ( _precedents )
			{
				if ( _status == StatusType.UPDATING )
				{
					if ( _precedents.Contains(precedent) )
						return false;
					_precedents.Add( precedent );
					return true;
				}
				else if ( _status != StatusType.UPDATING_AND_OUT_OF_DATE )
					Debug.Assert( false, "Unexpected state in AddPrecedent" );
				return false;
			}
		}

        static partial void ReportCycles();
    }
}
