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
using System.Diagnostics;

namespace UpdateControls
{
	/// <summary>
	/// A sentry that controls an independent field.
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
	/// <remarks>
	/// An independent field is one whose value can be changed externally at
	/// any time. Create one Independent sentry for each independent field in
	/// your object.
	/// </remarks>
	/// <example>A class using Independent sentries.
	/// <code language="C">
	///	public class Contact
	///	{
	///		private string _name = "";
	///		private string _emailAddress = "";
	///		
    ///		private Independent _indName = new Independent();
    ///		private Independent _indEmailAddress = new Independet();
	///
	///		public Contact()
	///		{
	///		}
	///
	///		public string Name
	///		{
	///			get
	///			{
	///				_indName.OnGet();
	///				return _name;
	///			}
	///			set
	///			{
	///				_indName.OnSet();
	///				_name = value;
	///			}
	///		}
	///
	///		public string EmailAddress
	///		{
	///			get
	///			{
	///				_indEmailAddress.OnGet();
	///				return _emailAddress;
	///			}
	///			set
	///			{
	///				_indEmailAddress.OnSet();
	///				_emailAddress = value;
	///			}
	///		}
	///	}
	/// </code>
    /// <code language="VB">
    ///	Public Class Contact
    ///		Private _name As String = ""
    ///		Private _emailAddress As String = ""
    ///
    ///		Private _indName As New Independent()
    ///		Private _indEmailAddress As New Independent()
    ///
    ///		Public Sub New()
    ///		End Sub
    ///
    ///		Public Property Name() As String
    ///			Get
    ///				_indName.OnGet()
    ///				Return _name
    ///			End Get
    ///			Set
    ///				_indName.OnSet()
    ///				_name = value
    ///			End Set
    ///		End Property
    ///
    ///		Public Property EmailAddress() As String
    ///			Get
    ///				_indEmailAddress.OnGet()
    ///				Return _emailAddress
    ///			End Get
    ///			Set
    ///				_indEmailAddress.OnSet()
    ///				_emailAddress = value
    ///			End Set
    ///		End Property
    ///	End Class
    /// </code>
	/// </example>
	public class Independent
	{
		private Precedent _base = new Precedent();

		/// <summary>
		/// Call this function just before getting the field that this
		/// sentry controls.
		/// </summary>
		/// <remarks>
		/// Any dependent fields that are currently updating will depend upon
		/// this field; when the field changes, the dependent becomes
		/// out-of-date.
		/// </remarks>
		public void OnGet()
		{
			// Establish dependency between the current update
			// and this field.
			_base.RecordDependent();
		}

		/// <summary>
		/// Call this function just before setting the field that this
		/// sentry controls.
		/// </summary>
		/// <remarks>
		/// Any dependent fields that depend upon this field will become
		/// out-of-date.
		/// </remarks>
		public void OnSet()
		{
			// Verify that dependents are not changing independents, as that
			// could be a logical circular dependency.
			if (Dependent.GetCurrentUpdate() != null)
				Debug.Assert(false, "An independent was changed while updating a dependent.");

			// When an independent field canges,
			// its dependents become out-of-date.
			_base.MakeDependentsOutOfDate();
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
				return _base.HasDependents;
			}
		}

		/// <summary>
        /// Event fired when the first dependent references this field. This event only
        /// fires when HasDependents goes from false to true. If the field already
        /// has dependents, then this event does not fire.
		/// </summary>
		public event Action GainDependent
		{
			add
			{
				_base.GainDependent += value;
			}
			remove
			{
				_base.GainDependent -= value;
			}
		}

		/// <summary>
        /// Event fired when the last dependent goes out-of-date. This event
        /// only fires when HasDependents goes from true to false. If the field has
        /// other dependents, then this event does not fire. If the dependent is
        /// currently updating and it still depends upon this field, then the
        /// GainDependent event will be fired immediately.
		/// </summary>
		public event Action LooseDependent
		{
			add
			{
				_base.LooseDependent += value;
			}
			remove
			{
				_base.LooseDependent -= value;
			}
		}
	}
}
