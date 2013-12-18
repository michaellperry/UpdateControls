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
	/// A date time picker that automatically updates its text.
	/// </summary>
	[Description("A date time picker that automatically updates its text."),
   ToolboxBitmap(typeof(UpdateDateTimePicker), "ToolboxImages.UpdateDateTimePicker.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetText")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateDateTimePicker : DateTimePicker, IEnabledControl
	{
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;
		/// <summary>
		/// Event fired to get the date and time selected.
		/// </summary>
		[Description("Event fired to get the date and time selected."),Category("Update")]
		public event GetDateTimeDelegate GetValue;
		/// <summary>
		/// Event fired with the selected date and time changes.
		/// </summary>
		[Description("Event fired with the selected date and time changes."),Category("Update")]
		public event SetDateTimeDelegate SetValue;

		private Dependent _depEnabled;
		private Dependent _depValue;
		private Independent _dynValue = Independent.New("UpdateDateTimePicker.Value");

		private int _updating = 0;

		/// <summary>
		/// Creates a new dependent date/time picker.
		/// </summary>
		public UpdateDateTimePicker()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateDateTimePicker.Enabled", UpdateEnabled);
			_depValue = Dependent.New("UpdateDateTimePicker.Value", UpdateValue);
		}

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

		private void UpdateValue()
		{
			++_updating;
			try
			{
				// Get the value from the event.
				if ( GetValue != null )
					base.Value = GetValue();
				else
					_dynValue.OnGet();
			}
			finally
			{
				--_updating;
			}
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
			_depValue.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depEnabled.OnGet();
			_depValue.OnGet();
		}

		/// <summary>
		/// Handle changes to the selected date.
		/// </summary>
		/// <param name="eventargs">unused</param>
		protected override void OnValueChanged(EventArgs eventargs)
		{
			if ( _updating == 0 )
			{
				if ( SetValue != null )
					SetValue( base.Value );
				_dynValue.OnSet();
			}
			base.OnValueChanged (eventargs);
		}

		/// <summary>
		/// Text displayed in the control (read-only).
		/// </summary>
        [Browsable(false)]
        public new string Text
		{
			get
			{
				_depValue.OnGet();
				return base.Text;
			}
            set { }
        }

		/// <summary>
		/// The selected date/time (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime Value
		{
			get
			{
				_depValue.OnGet();
				return base.Value;
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
