using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A check box that automatically updates its properties.
	/// </summary>
	[DefaultEvent("GetValue")]
	public class UpdateNumericUpDown : NumericUpDown, IEnabledControl
	{
		///  <summary>Event fired to get the current value of the NumericUpDown control.</summary>
		[Description("Event fired to get the current value of the NumericUpDown control."), Category("Update")]
		public event Func<decimal> GetValue;
		///  <summary>Event fired when the value is changed by the user.</summary>
		[Description("Event fired when the value is changed by the user."), Category("Update")]
		public event Action<decimal> SetValue;
		///  <summary>Event fired to get the minimum value of the NumericUpDown control.</summary>
		[Description("Event fired to get the minimum value of the NumericUpDown control."), Category("Update")]
		public event Func<decimal> GetMinimum;
		///  <summary>Event fired to get the maximum value of the NumericUpDown control.</summary>
		[Description("Event fired to get the maximum value of the NumericUpDown control."), Category("Update")]
		public event Func<decimal> GetMaximum;
		///  <summary>Event fired to get the step size of the NumericUpDown control.</summary>
		[Description("Event fired to get the step size of the NumericUpDown control."), Category("Update")]
		public event Func<decimal> GetIncrement;
		
		/// <summary>Event fired to determine whether the control is enabled.</summary>
		/// <remarks>
		/// Return true when the control should be enabled, and false when it should not be. If
		/// this event is not handled, the control is always enabled.
		/// </remarks>
		[Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

		private GuiUpdateHelper _helper;
		private int _updating = 0;
		private Dependent _depEnabled, _depValueAndRange, _depIncrement;

		/// <summary>
		/// Creates a new dependent trackbar.
		/// </summary>
		public UpdateNumericUpDown()
		{
			_depEnabled = Dependent.New("UpdateNumericUpDown.Enabled", UpdateEnabled);
			_depValueAndRange = Dependent.New("UpdateNumericUpDown.ValueAndRange", UpdateValueAndRange);
			_depIncrement = Dependent.New("UpdateNumericUpDown.Increment", UpdateIncrement);
			_helper = new GuiUpdateHelper(this, _depEnabled, _depValueAndRange, _depIncrement);
		}

		private void UpdateEnabled()
		{
			if (GetEnabled != null)
				base.Enabled = GetEnabled();
		}
		private void UpdateIncrement()
		{
			if (GetIncrement != null)
				base.Increment = GetIncrement();
		}
		
		// Note: Maximum, Minimum, and Value are set together to avoid glitches 
		// that would occur if there were three separate update methods for them.
		// For example, suppose at the beginning the range is 0-10 and the Value 
		// is 5. If the value and range suddenly change to 10-20 and 15, 
		// respectively, but the value is set before the range, it would have to 
		// be clipped to the old range (0-10), since the control will not allow 
		// Value to be outside the range.
		private void UpdateValueAndRange()
		{
			++_updating;
			try {

				decimal min = GetMinimum != null ? GetMinimum() : Minimum;
				decimal max = GetMaximum != null ? GetMaximum() : Maximum;
				// Note that if max < min, Maximum will end up set to Minimum
				base.Maximum = max;
				base.Minimum = min;

				if (GetValue != null)
				{
					decimal value = GetValue();
					if (min <= value && value <= max)
						base.Value = value;
					else if (value > max && max > min)
						base.Value = max;
					else
						base.Value = min;
				}
			} finally {
				--_updating;
			}
		}

		protected override void Dispose(bool disposing)
		{
			_helper.Dispose();
			base.Dispose(disposing);
		}

		protected override void OnValueChanged(EventArgs e)
		{
 			if (_updating == 0)
				if (SetValue != null)
					SetValue(Value);
			base.OnValueChanged(e);
		}

		[Browsable(false)]
		public new decimal Value
		{
			get {
				_depValueAndRange.OnGet();
				return base.Value;
			}
			set {
				AutoThrow(GetValue, "Value");
				base.Value = value;
			}
		}

		[Browsable(false)]
		public new bool Enabled
		{
			get {
				_depEnabled.OnGet();
				return base.Enabled;
			}
			set {
				AutoThrow(GetEnabled, "Enabled");
				base.Enabled = value;
			}
		}
		
		public new decimal Increment
		{
			get {
				_depIncrement.OnGet();
				return base.Increment;
			}
			set {
				AutoThrow(GetIncrement, "Increment");
				base.Increment = value;
			}
		}

		public new decimal Minimum
		{
			get {
				_depValueAndRange.OnGet();
				return base.Minimum;
			}
			set {
				AutoThrow(GetMinimum, "Minimum");
				base.Minimum = value;
			}
		}
		
		public new decimal Maximum
		{
			get {
				_depValueAndRange.OnGet();
				return base.Maximum;
			}
			set {
				AutoThrow(GetMaximum, "Maximum");
				base.Maximum = value;
			}
		}

		public void AutoThrow(object mustBeNull, string propertyName)
		{
			if (mustBeNull != null)
				throw new InvalidOperationException(
					string.Format("The {0}.{1} property cannot be set. It is under the control of the Get{1} event.", GetType().Name, propertyName));
		}
	}
}
