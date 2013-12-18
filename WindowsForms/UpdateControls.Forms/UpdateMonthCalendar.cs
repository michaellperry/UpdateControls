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
	/// A month calendar that automatically updates its properties.
	/// </summary>
	[Description("A month calendar that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateMonthCalendar), "ToolboxImages.UpdateMonthCalendar.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetSelectionStart")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateMonthCalendar : MonthCalendar, IEnabledControl
	{
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;
		/// <summary>
		/// Event fired to calculate the first selected date.
		/// </summary>
		[Description("Event fired to calculate the first selected date."),Category("Update")]
		public event GetDateTimeDelegate GetSelectionStart;
		/// <summary>
		/// Event fired to calculate the last selected date.
		/// </summary>
		[Description("Event fired to calculate the last selected date."),Category("Update")]
		public event GetDateTimeDelegate GetSelectionEnd;
		/// <summary>
		/// Event fired when the user changes the first selected date.
		/// </summary>
		[Description("Event fired when the user changes the first selected date."),Category("Update")]
		public event SetDateTimeDelegate SetSelectionStart;
		/// <summary>
		/// Event fired when the user changes the last selected date.
		/// </summary>
		[Description("Event fired when the user changes the last selected date."),Category("Update")]
		public event SetDateTimeDelegate SetSelectionEnd;
		/// <summary>
		/// Event fired to display the annually bolded dates.
		/// </summary>
		[Description("Event fired to display the annually bolded dates."),Category("Update")]
		public event GetDateTimeArrayDelegate GetAnnuallyBoldedDates;
		/// <summary>
		/// Event fired to display the monthly bolded dates.
		/// </summary>
		[Description("Event fired to display the monthly bolded dates."),Category("Update")]
		public event GetDateTimeArrayDelegate GetMonthlyBoldedDates;
		/// <summary>
		/// Event fired to display the bolded dates.
		/// </summary>
		[Description("Event fired to display the bolded dates."),Category("Update")]
		public event GetDateTimeArrayDelegate GetBoldedDates;

		private Dependent _depEnabled;
		private Dependent _depSelection;
		private Dependent _depAnnuallyBoldedDates;
		private Dependent _depMonthlyBoldedDates;
		private Dependent _depBoldedDates;

		private Independent _dynSelection = new Independent();

		private int _updating = 0;

		/// <summary>
		/// Creates a new dependent month calendar.
		/// </summary>
		public UpdateMonthCalendar()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateMonthCalendar.Enabled", UpdateEnabled);
			_depSelection = Dependent.New("UpdateMonthCalendar.Selection", UpdateSelection);
			_depAnnuallyBoldedDates = Dependent.New("UpdateMonthCalendar.AnuallyBoldedDates", UpdateAnnuallyBoldedDates);
			_depMonthlyBoldedDates = Dependent.New("UpdateMonthCalendar.MonthlyBoldedDates", UpdateMonthlyBoldedDates);
			_depBoldedDates = Dependent.New("UpdateMonthCalendar.BoldedDates", UpdateBoldedDates);
		}

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

		private void UpdateSelection()
		{
			++_updating;
			try
			{
				// Get the selection from the events.
				if ( GetSelectionStart != null || GetSelectionEnd != null )
				{
					if ( GetSelectionStart == null )
					{
						DateTime selected = GetSelectionEnd();
						base.SelectionStart = selected;
						base.SelectionEnd = selected;
					}
					else if ( GetSelectionEnd == null )
					{
						DateTime selected = GetSelectionStart();
						base.SelectionStart = selected;
						base.SelectionEnd = selected;
					}
					else
					{
						base.SelectionStart = GetSelectionStart();
						base.SelectionEnd = GetSelectionEnd();
					}
				}
				else
					_dynSelection.OnGet();
			}
			finally
			{
				--_updating;
			}
		}

		private void UpdateAnnuallyBoldedDates()
		{
			// Get the text from the event.
			if ( GetAnnuallyBoldedDates != null )
				base.AnnuallyBoldedDates = GetAnnuallyBoldedDates();
		}

		private void UpdateMonthlyBoldedDates()
		{
			// Get the text from the event.
			if ( GetMonthlyBoldedDates != null )
				base.MonthlyBoldedDates = GetMonthlyBoldedDates();
		}

		private new void UpdateBoldedDates()
		{
			// Get the text from the event.
			if ( GetBoldedDates != null )
				base.BoldedDates = GetBoldedDates();
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
			_depSelection.Dispose();
			_depAnnuallyBoldedDates.Dispose();
			_depMonthlyBoldedDates.Dispose();
			_depBoldedDates.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depEnabled.OnGet();
			_depSelection.OnGet();
			_depAnnuallyBoldedDates.OnGet();
			_depMonthlyBoldedDates.OnGet();
			_depBoldedDates.OnGet();
		}

		/// <summary>
		/// Handles changes to the selected date.
		/// </summary>
		/// <param name="drevent">Specifies the selected date.</param>
		protected override void OnDateChanged(DateRangeEventArgs drevent)
		{
			if ( _updating == 0 )
			{
				if ( SetSelectionStart != null )
					SetSelectionStart( base.SelectionStart );
				if ( SetSelectionEnd != null )
					SetSelectionEnd( base.SelectionEnd );
				_dynSelection.OnSet();
			}
			base.OnDateChanged (drevent);
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
		/// Range of dates that are currently selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new SelectionRange SelectionRange
		{
			get
			{
				_depSelection.OnGet();
				return base.SelectionRange;
			}
            set { }
        }

		/// <summary>
		/// First date that is currently selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime SelectionStart
		{
			get
			{
				_depSelection.OnGet();
				return base.SelectionStart;
            }
            set { }
		}

		/// <summary>
		/// Last date that is currently selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime SelectionEnd
		{
			get
			{
				_depSelection.OnGet();
				return base.SelectionEnd;
			}
            set { }
        }

		/// <summary>
		/// Collection of dates that are bolded in each year (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime[] AnnuallyBoldedDates
		{
			get
			{
				_depAnnuallyBoldedDates.OnGet();
				return base.AnnuallyBoldedDates;
			}
            set { }
        }

		/// <summary>
		/// Collection of dates that are bolded in each month (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime[] MonthlyBoldedDates
		{
			get
			{
				_depMonthlyBoldedDates.OnGet();
				return base.MonthlyBoldedDates;
			}
            set { }
        }

		/// <summary>
		/// Collection of dates that are bolded (read-only).
		/// </summary>
        [Browsable(false)]
        public new DateTime[] BoldedDates
		{
			get
			{
				_depBoldedDates.OnGet();
				return base.BoldedDates;
			}
            set { }
        }
	}
}
