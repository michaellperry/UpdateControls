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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A list box that automatically updates its properties.
	/// </summary>
	[Description("A list box that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateListBox), "ToolboxImages.UpdateListBox.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItems")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateListBox : ListBox, IEnabledControl
	{
		private class ListBoxItem : IDisposable
		{
			private object _tag;
			private string _text;
			private bool _selected;
			private Dependent _depText;
			private Dependent _depSelected;
			private GetObjectStringDelegate _getItemText;
			private GetObjectBoolDelegate _getItemSelected;
			private SetObjectBoolDelegate _setItemSelected;

			public ListBoxItem( object tag, GetObjectStringDelegate getItemText, GetObjectBoolDelegate getItemSelected, SetObjectBoolDelegate setItemSelected )
			{
				_tag = tag;
				_getItemText = getItemText;
				_getItemSelected = getItemSelected;
				_setItemSelected = setItemSelected;
				_depText = Dependent.New("ListBoxItem.Text", UpdateText);
				_depSelected = Dependent.New("ListBoxItem.Selected", UpdateSelected);
			}

			public void Dispose()
			{
				_depText.Dispose();
				_depSelected.Dispose();
			}

			public object Tag
			{
				get
				{
					return _tag;
				}
			}

			public bool Selected
			{
				get
				{
					_depSelected.OnGet();
					return _selected;
				}
				set
				{
					if ( _selected != value && _setItemSelected != null )
						_setItemSelected( _tag, value );
				}
			}

			private void UpdateText()
			{
				_text =
					_getItemText == null ?
						_tag == null ?
							"" :
							_tag.ToString() :
						_getItemText( _tag );
				if ( _text == null )
					_text = "";
			}

			private void UpdateSelected()
			{
				_selected =
					_getItemSelected == null ?
						false :
						_getItemSelected( _tag );
			}

			public override bool Equals( object obj )
			{
				if ( obj.GetType() != GetType() )
					return false;
				ListBoxItem that = (ListBoxItem)obj;
				return Object.Equals( _tag, that._tag );
			}

			public override int GetHashCode()
			{
				return _tag == null ?
					0 :
					_tag.GetHashCode();
			}

			public override string ToString()
			{
				_depText.OnGet();
				return _text;
			}
		}

        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;
		/// <summary>
		/// Event fired to get the list of items.
		/// </summary>
		[Description("Event fired to get the list of items."),Category("Update")]
		public event GetCollectionDelegate GetItems;
		/// <summary>
		/// Event fired to get the text associated with an item.
		/// </summary>
		[Description("Event fired to get the text associated with an item."),Category("Update")]
		public new event GetObjectStringDelegate GetItemText;
		/// <summary>
		/// Event fired to get the selected item of a single-select list box.
		/// </summary>
		[Description("Event fired to get the selected item of a single-select list box."),Category("Update")]
		public event GetObjectDelegate GetSelectedItem;
		/// <summary>
		/// Event fired when the user selects an item of a single-select list box.
		/// </summary>
		[Description("Event fired when the user selects an item of a single-select list box."),Category("Update")]
		public event SetObjectDelegate SetSelectedItem;
		/// <summary>
		/// Event fired to get the selected items of a multi-select list box.
		/// </summary>
		[Description("Event fired to get the selected items of a multi-select list box."),Category("Update")]
		public event GetObjectBoolDelegate GetItemSelected;
		/// <summary>
		/// Event fired when the user selects items of a multi-select list box.
		/// </summary>
		[Description("Event fired when the user selects items of a multi-select list box."),Category("Update")]
		public event SetObjectBoolDelegate SetItemSelected;

		private Dependent _depEnabled;
		private Dependent _depItems;
		private Dependent _depSelectedItem;

		private Independent _dynSelectedItem = Independent.New("UpdateListBox.SelectedItem");

        private int _updating = 0;

		/// <summary>
		/// Creates a new dependent list box.
		/// </summary>
		public UpdateListBox()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateListBox.Enabled", UpdateEnabled );
			_depItems = Dependent.New("UpdateListBox.Items", UpdateItems );
			_depSelectedItem = Dependent.New("UpdateListBox.SelectedItem", UpdateSelectedItem );
		}

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

        private void UpdateItems()
        {
            ++_updating;
            try
            {
                if (GetItems != null)
                {
                    // Replace the items in the control.
                    base.BeginUpdate();
                    try
                    {
                        // Make sure the same tag is selected.
                        ListBoxItem selectedItem = (ListBoxItem)base.SelectedItem;
                        object selectedTag = selectedItem == null ? null : selectedItem.Tag;
                        Util.CollectionHelper.RecycleCollection(
                            base.Items,
                            GetItems().OfType<object>().Select(item =>
                                new ListBoxItem(item, GetItemText, GetItemSelected, SetItemSelected)));
                        base.SelectedIndex = IndexOfTag(selectedTag);
                    }
                    finally
                    {
                        base.EndUpdate();
                    }
                }
            }
            finally
            {
                --_updating;
            }
        }

        private int IndexOfTag(object tag)
		{
			if ( tag == null )
				return -1;

			// Scan for the tag.
			int index = 0;
			foreach ( ListBoxItem item in base.Items )
			{
				if ( Object.Equals( item.Tag, tag ) )
					return index;
				++index;
			}

			// Not found.
			return -1;
		}

		private void UpdateSelectedItem()
		{
			++_updating;
			try 
			{
				_depItems.OnGet();

				if ( base.SelectionMode == SelectionMode.One )
				{
					// Honor single selection.
					if ( GetSelectedItem != null )
					{
						// If the event is defined, use it to get the selected item.
						base.SelectedIndex = IndexOfTag( GetSelectedItem() );
					}
					else
					{
						_dynSelectedItem.OnGet();
					}
				}
				else if ( base.SelectionMode == SelectionMode.MultiSimple ||
					base.SelectionMode == SelectionMode.MultiExtended )
				{
					// Honor multiple selection.
					if ( GetItemSelected != null )
					{
						// If the event is defined, use it to get the selected items.
						base.BeginUpdate();
						try
						{
							int index = 0;
							ArrayList items = new ArrayList( base.Items );
							foreach ( ListBoxItem item in items )
							{
								bool selected = item.Selected;
								if ( base.GetSelected( index ) != selected )
									base.SetSelected( index, selected );
								++index;
							}
						}
						finally
						{
							base.EndUpdate();
						}
					}
					else
					{
						_dynSelectedItem.OnGet();
					}
				}
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
			_depItems.Dispose();
			_depSelectedItem.Dispose();
			foreach ( ListBoxItem item in base.Items )
				item.Dispose();
			base.OnHandleDestroyed (e);
		}

        private void Application_Idle(object sender, EventArgs e)
        {
            // Update all dependent sentries.
            if (!this.Capture)
            {
                _depEnabled.OnGet();
                _depItems.OnGet();
                _depSelectedItem.OnGet();
            }
        }

		/// <summary>
		/// Handle changes to the selected items.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnSelectedIndexChanged( EventArgs e )
		{
			// Respond to selection changes, as long as they aren't updates.
			if ( _updating == 0 )
			{
				if ( base.SelectionMode == SelectionMode.One )
				{
					// Set the one selected item.
					ListBoxItem item = (ListBoxItem)base.SelectedItem;
					if ( SetSelectedItem != null )
						SetSelectedItem( item == null ? null : item.Tag );
					_dynSelectedItem.OnSet();
				}
				else if ( base.SelectionMode == SelectionMode.MultiSimple ||
					base.SelectionMode == SelectionMode.MultiExtended )
				{
					// Record the selected items.
					if ( SetItemSelected != null )
					{
						int index = 0;
						foreach ( ListBoxItem item in base.Items )
						{
							item.Selected = base.GetSelected( index );
							++index;
						}
					}
					_dynSelectedItem.OnSet();
				}
			}

			base.OnSelectedIndexChanged( e );
		}

		/// <summary>
		/// Text displayed in the control (read-only).
		/// </summary>
        [Browsable(false)]
        public new string Text
		{
			get
			{
				_depSelectedItem.OnGet();
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

		private static object Map( object source )
		{
			return ((ListBoxItem)source).Tag;
		}

		/// <summary>
		/// Collection of items in the list box (read-only).
		/// </summary>
        [Browsable(false)]
        public new IList Items
		{
			get
			{
				_depItems.OnGet();
				return new UpdateControls.Forms.Util.ReadOnlyListDecorator(
					base.Items,
					new UpdateControls.Forms.Util.MapDelegate(Map));
			}
        }

		/// <summary>
		/// The items in the single-selection list box that is selected.
		/// </summary>
        [Browsable(false)]
        public new object SelectedItem
		{
			get
			{
				_depSelectedItem.OnGet();
				ListBoxItem selectedItem = (ListBoxItem)base.SelectedItem;
				return selectedItem == null ? null : selectedItem.Tag;
			}
			set
			{
				_depItems.OnGet();
				if ( base.SelectionMode == SelectionMode.One )
				{
					base.SelectedIndex = IndexOfTag( value );
					if ( SetSelectedItem != null )
						SetSelectedItem( value );
					_dynSelectedItem.OnSet();
				}
			}
		}

		/// <summary>
		/// Index of the selected item of a single-selection list box.
		/// </summary>
        [Browsable(false)]
        public new int SelectedIndex
		{
			get
			{
				_depSelectedItem.OnGet();
				return base.SelectedIndex;
			}
			set
			{
				_depItems.OnGet();
				if ( base.SelectionMode == SelectionMode.One )
				{
					base.SelectedIndex = value;
					if ( SetSelectedItem != null )
						SetSelectedItem( base.SelectedItem );
					_dynSelectedItem.OnSet();
				}
			}
		}

		/// <summary>
		/// Collection of items in the multi-select list box that are selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new IList SelectedItems
		{
			get
			{
				_depSelectedItem.OnGet();
				return new UpdateControls.Forms.Util.ReadOnlyListDecorator(
					base.SelectedItems,
					new UpdateControls.Forms.Util.MapDelegate(Map));
			}
        }

		/// <summary>
		/// Indices of the items of a multi-select list box that are selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new ListBox.SelectedIndexCollection SelectedIndices
		{
			get
			{
				_depSelectedItem.OnGet();
				return base.SelectedIndices;
			}
        }
	}
}
