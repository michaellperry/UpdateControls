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
using System.Drawing;
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A button that automatically updates its properties.
	/// </summary>
	[Description("A button that automatically updates its properties."),
    ToolboxBitmap(typeof(UpdateButton), "ToolboxImages.UpdateButton.png"),
	DefaultProperty("Name"),
	DefaultEvent("Click")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateButton : Button, IEnabledControl
	{
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private Dependent _depEnabled;

		/// <summary>
		/// Constructs a new dependent button.
		/// </summary>
		public UpdateButton()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateButton.Enabled", UpdateEnabled);
		}

		/// <summary>
		/// This member overrides <see cref="Control.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

		/// <summary>
		/// Register idle-time updates for the control.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
			base.OnHandleCreated (e);
		}

		/// <summary>
		/// Unregister idle-time updates for the control.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Unregister idle-time updates.
			Application.Idle -= new EventHandler(Application_Idle);
			_depEnabled.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depEnabled.OnGet();
		}

        /// <summary>True if the control is enabled (read-only).</summary>
        /// <remarks>
        /// To enable or disable the control, handle the <see cref="GetEnabled"/>
        /// event. This property cannot be set directly.
        /// </remarks>
        [Browsable(false)]
        public new bool Enabled
		{
			get
			{
				_depEnabled.OnGet();
				return base.Enabled;
			}
            set { }
        }
	}
}
