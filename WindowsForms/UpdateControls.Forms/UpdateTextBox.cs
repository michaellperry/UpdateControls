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
	/// A text box that automatically updates its text.
	/// </summary>
	[Description("A text box that automatically updates its text."),
    ToolboxBitmap(typeof(UpdateTextBox), "ToolboxImages.UpdateTextBox.png"),
	 DefaultProperty("Name"),
	 DefaultEvent("GetText")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateTextBox : TextBox, IEnabledControl, IErrorControl
	{
		/// <summary>
		/// Event fired to calculate the text to display.
		/// </summary>
		[Description("Event fired to calculate the text to display."),Category("Update")]
		public event GetStringDelegate GetText;
		/// <summary>
		/// Event fired when the displayed text changes.
		/// </summary>
		[Description("Event fired when the displayed text changes."),Category("Update")]
		public event SetStringDelegate SetText;
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private Dependent _depText;
		private Dependent _depEnabled;
		private Independent _dynModified = Independent.New("UpdateTextBox.Modified");
		private Independent _dynError = Independent.New("UpdateTextBox.Error");

		private bool _realTime = false;
        private string _errorText = string.Empty;

		private int _updating = 0;

		/// <summary>
		/// Creates a new dependent text box.
		/// </summary>
		public UpdateTextBox()
		{
            // Create all dependent sentries.
			_depText = Dependent.New("UpdateTextBox.Text", UpdateText);
			_depEnabled = Dependent.New("UpdateTextBox.Enabled", UpdateEnabled);
		}

		private void UpdateText()
		{
			// See if the user is typing.
			_dynModified.OnGet();
            _dynError.OnGet();
			if ( !base.Modified && string.IsNullOrEmpty(_errorText) )
			{
				// Get the text from the event.
				++_updating;
				try 
				{
					if ( GetText != null )
						base.Text = GetText();
					base.SelectAll();
				}
				finally
				{
					--_updating;
				}
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
		/// Handle changes to text.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnTextChanged(EventArgs e)
		{
			// Respond to text changes, as long as they aren't updates.
			if ( _updating == 0 )
			{
				_dynModified.OnSet();
                if (_realTime && SetText != null)
                {
                    CallSetText();
                }
			}

			base.OnTextChanged( e );
		}

		/// <summary>
		/// Give control back to the dependency system.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnLeave(EventArgs e)
		{
			if ( base.Modified )
			{
				// Respond to text changes, as long as they aren't updates.
				if ( _updating == 0 && !_realTime && SetText != null )
					CallSetText();

				_dynModified.OnSet();
				base.Modified = false;
			}

			base.OnLeave( e );
		}

        private void CallSetText()
        {
            string errorText = string.Empty;
            try
            {
                SetText(base.Text);
            }
            catch (Exception x)
            {
                // If SetText threw, then store the error text.
                errorText = x.Message;
                if (string.IsNullOrEmpty(errorText))
                    errorText = x.GetType().Name;
            }

            if (errorText != _errorText)
            {
                _dynError.OnSet();
                _errorText = errorText;
            }
        }

		/// <summary>
		/// Text displayed in the control (read-only).
		/// </summary>
        [Browsable(false)]
        public override string Text
		{
			get
			{
                if ( _updating == 0 )
    				_depText.OnGet();
				return base.Text;
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
		/// True if the control responds to changes immediately.
		/// False if it waits until the focus has left the control.
		/// </summary>
        [Description("True if the control responds to changes immediately. False if it waits until the focus has left the control."),
        Category("Update"), DefaultValue(false)]
		public bool RealTime
		{
			get
			{
				return _realTime;
			}
			set
			{
				_realTime = value;
			}
		}

        #region IErrorControl Members

        public string GetError()
        {
            _dynError.OnGet();
            return _errorText;
        }

        #endregion
    }
}
