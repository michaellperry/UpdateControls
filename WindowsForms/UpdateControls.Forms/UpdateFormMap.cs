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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A map of objects to forms that automatically closes when objects are deleted.
	/// </summary>
	[Description("A map of objects to forms that automatically closes when objects are deleted."),
   ToolboxBitmap(typeof(UpdateFormMap), "ToolboxImages.UpdateFormMap.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetObjectExists")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateFormMap : System.ComponentModel.Component
	{
		private class Entry : IDisposable
		{
			private object _tag;
			private Form _form;
			private GetObjectBoolDelegate _getObjectExists;

			private Dependent _depExists;

			public Entry( object tag, Form form, GetObjectBoolDelegate getObjectExists )
			{
				_tag = tag;
				_form = form;
				_getObjectExists = getObjectExists;
				_depExists = new Dependent( UpdateExists );

				// Clean up when the form is closed.
				form.Closed += new EventHandler(form_Closed);
			}

			public Form Form
			{
				get
				{
					_depExists.OnGet();
					return _form;
				}
			}

			private void UpdateExists()
			{
				if ( _getObjectExists != null && _form != null )
				{
					if ( !_getObjectExists( _tag ) )
					{
						_form.Close();
						_form = null;
					}
				}
			}

			private void form_Closed(object sender, EventArgs e)
			{
				_form = null;
			}

			public void Dispose()
			{
				if( _form != null )
					_form.Dispose();
				_depExists.Dispose();
			}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Event fired to determine whether an object exists.
		/// </summary>
		[Description("Event fired to determine whether an object exists."),Category("Update")]
		public event GetObjectBoolDelegate GetObjectExists;
		/// <summary>
		/// Event fired to create a form for an object.
		/// </summary>
		[Description("Event fired to create a form for an object."),Category("Update")]
		public event CreateFormDelegate CreateForm;

		private Hashtable _formTable = new Hashtable();

		private Dependent _depForms;
		private Independent _dynFormTable = new Independent();

		/// <summary>
		/// Creates a new dependent form map inside a container.
		/// </summary>
		/// <param name="container">The container to which to add the dependent form map.</param>
		public UpdateFormMap(System.ComponentModel.IContainer container)
		{
            container.Add(this);
			InitializeComponent();

			// Create all dependent sentries.
			_depForms = new Dependent( UpdateForms );

			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
		}

		/// <summary>
		/// Creates a new dependent form map without adding it to a container.
		/// </summary>
		public UpdateFormMap()
		{
			InitializeComponent();

			// Create all dependent sentries.
			_depForms = new Dependent( UpdateForms );

			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
		}

		private void UpdateForms()
		{
			// Prune the form table.
			_dynFormTable.OnGet();
			ArrayList tags = new ArrayList( _formTable.Keys );
			foreach ( object tag in tags )
			{
				Entry entry = (Entry)_formTable[ tag ];
				if ( entry != null && entry.Form == null )
					_formTable.Remove( tag );
			}
		}

		/// <summary>
		/// Display a form for editing an object. If such a form was
		/// already created, it is brought to the top. If not, a new form
		/// is created.
		/// </summary>
		/// <param name="tag">The object to be edited</param>
		public void ShowForm( object tag )
		{
			// First see if the tag already has a form.
			Entry entry = (Entry)_formTable[ tag ];
			if ( entry != null && entry.Form != null )
				entry.Form.BringToFront();
			else if ( CreateForm != null )
			{
				// It doesn't so create the form and add it to the set.
				Form form = CreateForm( tag );
				if ( form != null )
				{
					entry = new Entry( tag, form, GetObjectExists );
					if ( entry.Form != null )
					{
						_dynFormTable.OnSet();
						_formTable[ tag ] = entry;
						form.Show();
					}
				}
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                Application.Idle -= new EventHandler(Application_Idle);

				if(components != null)
				{
					components.Dispose();
				}
				if ( _formTable != null )
				{
					foreach ( Entry entry in _formTable.Values )
						entry.Dispose();
					_formTable = null;
				}
				_depForms.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		private void Application_Idle(object sender, EventArgs e)
		{
            _depForms.OnGet();
		}
	}
}
