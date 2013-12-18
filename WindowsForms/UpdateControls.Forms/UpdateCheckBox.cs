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
	/// A check box that automatically updates its properties.
	/// </summary>
	[Description("A check box that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateCheckBox), "ToolboxImages.UpdateCheckBox.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetChecked")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateCheckBox : CheckBox, IEnabledControl
	{
		/// <summary>
		/// Event fired to calculate whether the box is checked.
		/// </summary>
		[Description("Event fired to calculate whether the box is checked."),Category("Update")]
		public event GetBoolDelegate GetChecked;
		/// <summary>
		/// Event fired when the box is checked or unchecked.
		/// </summary>
		[Description("Event fired when the box is checked or unchecked."),Category("Update")]
		public event SetBoolDelegate SetChecked;
		/// <summary>
		/// Event fired to calculate the check state of the three-state box.
		/// </summary>
		[Description("Event fired to calculate the check state of the three-state box."),Category("Update")]
		public event GetCheckStateDelegate GetCheckState;
		/// <summary>
		/// Event fired when the state of the three-state box is changed.
		/// </summary>
		[Description("Event fired when the state of the three-state box is changed."),Category("Update")]
		public event SetCheckStateDelegate SetCheckState;
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private Dependent _depChecked;
		private Dependent _depEnabled;

		private Independent _dynChecked = Independent.New("UpdateCheckBox.Checked");

		private int _updating = 0;

		/// <summary>
		/// Creates a new dependent check box.
		/// </summary>
		public UpdateCheckBox()
		{
			// Create all dependent sentries.
			_depChecked = Dependent.New("UpdateCheckBox.Checked", UpdateChecked);
			_depEnabled = Dependent.New("UpdateCheckBox.Enabled", UpdateEnabled);
		}

		private void UpdateChecked()
		{
			++_updating;
			try
			{
				// Get the check state from the event.
				if ( base.ThreeState )
				{
					// Check state takes precedence.
					if ( GetCheckState != null )
						base.CheckState = GetCheckState();
					else if ( GetChecked != null )
						base.Checked = GetChecked();
					else
						_dynChecked.OnGet();
				}
				else
				{
					// Checked takes precedence.
					if ( GetChecked != null )
						base.Checked = GetChecked();
					else if ( GetCheckState != null )
						base.CheckState = GetCheckState();
					else
						_dynChecked.OnGet();
				}
			}
			finally
			{
				--_updating;
			}
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
			_depChecked.Dispose();
			_depEnabled.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depChecked.OnGet();
			_depEnabled.OnGet();
		}

		/// <summary>
		/// Handle changes to check state.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnCheckStateChanged(EventArgs e)
		{
			if ( _updating == 0 )
			{
				// Set the check state if available.
				if ( SetCheckState != null )
					SetCheckState( base.CheckState );
				// Set checked if available.
				if ( SetChecked != null )
					SetChecked( base.Checked );
				_dynChecked.OnSet();
			}

			base.OnCheckStateChanged( e );
		}

		/// <summary>
		/// True if the check box is checked (read-only).
		/// </summary>
        [Browsable(false)]
        public new bool Checked
		{
			get
			{
				_depChecked.OnGet();
				return base.Checked;
			}
            set { }
        }

		/// <summary>
		/// The state of the three-state check box (read-only).
		/// </summary>
        [Browsable(false)]
        public new CheckState CheckState
		{
			get
			{
				_depChecked.OnGet();
				return base.CheckState;
			}
            set { }
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
