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
	/// A label that automatically updates its properties.
	/// </summary>
	[Description("A label that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateLabel), "ToolboxImages.UpdateLabel.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetText")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateLabel : Label, IEnabledControl
	{
		/// <summary>
		/// Event fired to calculate the text to display.
		/// </summary>
		[Description("Event fired to calculate the text to display."),Category("Update")]
		public event GetStringDelegate GetText;
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private Dependent _depText;
		private Dependent _depEnabled;

        private int _updating = 0;

		/// <summary>
		/// Creates a new dependent label.
		/// </summary>
		public UpdateLabel()
		{
            // Create all dependent sentries.
			_depText = Dependent.New("UpdateLabel.Text", UpdateText);
			_depEnabled = Dependent.New("UpdateLabel.Enabled", UpdateEnabled);
		}

		private void UpdateText()
		{
            ++_updating;
            try
            {
                // Get the text from the event.
                if (GetText != null)
                    base.Text = GetText();
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
			_depText.Dispose();
			_depEnabled.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depText.OnGet();
			_depEnabled.OnGet();
		}

		/// <summary>
		/// Text displayed in the control (read-only).
		/// </summary>
        [Browsable(false)]
		public override string Text
		{
			get
			{
                if (_updating == 0)
                {
                    if (GetText != null)
                    {
                        _depText.OnGet();
                        return base.Text;
                    }
                    else
                        return "updateLabel";
                }
                else
                {
                    return base.Text;
                }
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
