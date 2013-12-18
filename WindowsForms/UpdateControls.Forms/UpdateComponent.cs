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
using System.Drawing;
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A component that automatically updates any property.
	/// </summary>
	[Description("A component that automatically updates any property."),
   ToolboxBitmap(typeof(UpdateComponent), "ToolboxImages.UpdateComponent.png"),
	DefaultProperty("Name"),
	DefaultEvent("Update")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateComponent : System.ComponentModel.Component
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Event fired to update the property.
		/// </summary>
		[Description("Event fired to update the property."),Category("Update")]
		public event ActionDelegate Update;

		private Dependent _depUpdate;

		/// <summary>
		/// Creates a new dependent component inside a container.
		/// </summary>
		/// <param name="container">The container to which to add the component.</param>
		public UpdateComponent(System.ComponentModel.IContainer container)
		{
			container.Add(this);
			InitializeComponent();

			// Create all dependent sentries.
			_depUpdate = new Dependent( DoUpdate );

			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
		}

		/// <summary>
		/// Creates a new dependent component without adding it to a container.
		/// </summary>
		public UpdateComponent()
		{
            InitializeComponent();

			// Create all dependent sentries.
			_depUpdate = new Dependent( DoUpdate );

			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
		}

		private void DoUpdate()
		{
			// Execute the update event.
			if ( Update != null )
				Update();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}

				_depUpdate.Dispose();

				// Unregister idle-time updates.
				Application.Idle -= new EventHandler(Application_Idle);
			}
			base.Dispose( disposing );
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depUpdate.OnGet();
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
	}
}
