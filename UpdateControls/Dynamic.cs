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

namespace UpdateControls
{
	/// <summary>
	/// A sentry that controls a dynamic field.
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
	/// <remarks>
	/// A dynamic field is one whose value can be changed externally at
	/// any time. Create one Dynamic sentry for each dynamic field in
	/// your object.
	/// </remarks>
	/// <example>A class using Dynamic sentries.
	/// <code language="C">
	///	public class Contact
	///	{
	///		private string _name = "";
	///		private string _emailAddress = "";
	///		
	///		private Dynamic _dynName = new Dynamic();
	///		private Dynamic _dynEmailAddress = new Dynamic();
	///
	///		public Contact()
	///		{
	///		}
	///
	///		public string Name
	///		{
	///			get
	///			{
	///				_dynName.OnGet();
	///				return _name;
	///			}
	///			set
	///			{
	///				_dynName.OnSet();
	///				_name = value;
	///			}
	///		}
	///
	///		public string EmailAddress
	///		{
	///			get
	///			{
	///				_dynEmailAddress.OnGet();
	///				return _emailAddress;
	///			}
	///			set
	///			{
	///				_dynEmailAddress.OnSet();
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
    ///		Private _dynName As New Dynamic()
    ///		Private _dynEmailAddress As New Dynamic()
    ///
    ///		Public Sub New()
    ///		End Sub
    ///
    ///		Public Property Name() As String
    ///			Get
    ///				_dynName.OnGet()
    ///				Return _name
    ///			End Get
    ///			Set
    ///				_dynName.OnSet()
    ///				_name = value
    ///			End Set
    ///		End Property
    ///
    ///		Public Property EmailAddress() As String
    ///			Get
    ///				_dynEmailAddress.OnGet()
    ///				Return _emailAddress
    ///			End Get
    ///			Set
    ///				_dynEmailAddress.OnSet()
    ///				_emailAddress = value
    ///			End Set
    ///		End Property
    ///	End Class
    /// </code>
	/// </example>
	public class Dynamic
	{
		private Precedent _base = new Precedent();

		/// <summary>
		/// Call this function just before getting the field that this
		/// sentry controls.
		/// </summary>
		/// <remarks>
		/// Any dependent fields that are currently updating will depend upon
		/// this dynamic; when the dynamic changes, the dependent becomes
		/// out-of-date.
		/// </remarks>
		public void OnGet()
		{
			// Establish dependency between the current update
			// and this attribute.
			_base.RecordDependent();
		}

		/// <summary>
		/// Call this function just before setting the field that this
		/// sentry controls.
		/// </summary>
		/// <remarks>
		/// Any dependent fields that depend upon this dynamic will become
		/// out-of-date.
		/// </remarks>
		public void OnSet()
		{
			// When a dynamic attribute canges,
			// its dependents become out-of-date.
			_base.MakeDependentsOutOfDate();
		}

		/// <summary>
		/// True if any other fields depend upon this one.
		/// </summary>
		/// <remarks>
		/// If any dependent field has used this dynamic field while updating,
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
		/// </summary>
		public event Notice GainDependent
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
		/// </summary>
		public event Notice LooseDependent
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
