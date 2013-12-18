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
	/// A radio button that automatically updates its properties.
	/// </summary>
	[Description("A radio button that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateRadioButton), "ToolboxImages.UpdateRadioButton.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetChecked")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateRadioButton : RadioButton, IEnabledControl
	{
		/// <summary>
		/// Event fired to calculate whether the button is checked.
		/// </summary>
		[Description("Event fired to calculate whether the button is checked."),Category("Update")]
		public event GetBoolDelegate GetChecked;
		/// <summary>
		/// Event fired when the button is checked.
		/// </summary>
		[Description("Event fired when the button is checked."),Category("Update")]
		public event ActionDelegate SetChecked;
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private Dependent _depChecked;
		private Dependent _depEnabled;

		/// <summary>
		/// Creates a new dependent radio button.
		/// </summary>
		public UpdateRadioButton()
		{
            // Create all dependent sentries.
			_depChecked = Dependent.New("UpdateRadioButton.Checked", UpdateChecked);
			_depEnabled = Dependent.New("UpdateRadioButton.Enabled", UpdateEnabled);

			// Turn off AutoCheck by default.
			base.AutoCheck = false;
		}

		private void UpdateChecked()
		{
			// Get the text from the event.
			if ( GetChecked != null )
			{
				bool isChecked = GetChecked();
				base.Checked = isChecked;
				base.TabStop = isChecked;
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
		/// Handles mouse clicks.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnClick(EventArgs e)
		{
			if ( SetChecked != null )
			{
				SetChecked();
			}

			base.OnClick( e );
		}

		/// <summary>
		/// True if the radio button is checked (read-only).
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

		/// <summary>
		/// True if the radio button is a tab-stop (read-only).
		/// </summary>
        [Browsable(false)]
        public new bool TabStop
		{
			get
			{
				_depChecked.OnGet();
				return base.TabStop;
			}
            set { }
        }

        [Browsable(false), DefaultValue(false)]
        public new bool AutoCheck
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
	}
}
