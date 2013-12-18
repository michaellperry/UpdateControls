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
using UpdateControls.Fields;

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
	public class Independent : Precedent
	{
		public static Independent New() { return DebugMode ? new NamedIndependent() : new Independent(); }
		public static NamedIndependent New(string name) { return new NamedIndependent(name); }
		public static Independent<T> New<T>(string name) { return new Independent<T>(name, default(T)); }
		public static NamedIndependent New(Type containerType, string name) { return new NamedIndependent(containerType, name); }
		public static Independent<T> New<T>(Type containerType, string name) { return new Independent<T>(containerType, name); }

		/// <summary>
		/// Call this function just before getting the field that this
		/// sentry controls.
		/// </summary>
		/// <remarks>
		/// Any dependent fields that are currently updating will depend upon
		/// this field; when the field changes, the dependent becomes
		/// out-of-date.
		/// </remarks>
		public virtual void OnGet()
		{
			// Establish dependency between the current update
			// and this field.
			RecordDependent();
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

			// When an independent field changes,
			// its dependents become out-of-date.
			MakeDependentsOutOfDate();
		}

		/// <summary>Intended for the debugger. Returns a tree of Dependents that 
		/// use this Dependent.</summary>
		protected DependentVisualizer UsedBy
		{
			get { return new DependentVisualizer(this); }
		}
	}
}
