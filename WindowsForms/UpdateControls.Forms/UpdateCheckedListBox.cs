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
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A checked list box that automatically updates its properties.
	/// </summary>
	[Description("A checked list box that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateCheckedListBox), "ToolboxImages.UpdateCheckedListBox.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItemChecked")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateCheckedListBox : CheckedListBox, IEnabledControl
	{
		private class CheckedListBoxItem : IDisposable
		{
			private object _tag;
			private string _text;
			private CheckState _checkState;
			private Dependent _depText;
			private Dependent _depCheckState;
			private GetObjectStringDelegate _getItemText;
			private GetObjectCheckStateDelegate _getItemCheckState;
			private SetObjectCheckStateDelegate _setItemCheckState;

			public CheckedListBoxItem( object tag, GetObjectStringDelegate getItemText, GetObjectCheckStateDelegate getItemCheckState, SetObjectCheckStateDelegate setItemCheckState )
			{
				_tag = tag;
				_getItemText = getItemText;
				_getItemCheckState = getItemCheckState;
				_setItemCheckState = setItemCheckState;
				_depText = Dependent.New("CheckedListBoxItem.Text", UpdateText );
				_depCheckState = Dependent.New("CheckedListBoxItem.CheckState", UpdateCheckState);
			}

			public void Dispose()
			{
				_depText.Dispose();
				_depCheckState.Dispose();
			}

			public object Tag
			{
				get
				{
					return _tag;
				}
			}

			public CheckState CheckState
			{
				get
				{
					_depCheckState.OnGet();
					return _checkState;
				}
				set
				{
					if ( _checkState != value && _setItemCheckState != null )
						_setItemCheckState( _tag, value );
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

			private void UpdateCheckState()
			{
				_checkState =
					_getItemCheckState == null ?
						CheckState.Unchecked :
						_getItemCheckState( _tag );
			}

			public override bool Equals( object obj )
			{
				if ( obj.GetType() != GetType() )
					return false;
				CheckedListBoxItem that = (CheckedListBoxItem)obj;
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
		/// Event fired to determine whether an item is checked.
		/// </summary>
		[Description("Event fired to determine whether an item is checked."),Category("Update")]
		public new event GetObjectBoolDelegate GetItemChecked;
		/// <summary>
		/// Event fired when an item is checked or unchecked.
		/// </summary>
		[Description("Event fired when an item is checked or unchecked."),Category("Update")]
		public new event SetObjectBoolDelegate SetItemChecked;
		/// <summary>
		/// Event fired to determine the check state of a three-state item.
		/// </summary>
		[Description("Event fired to determine the check state of a three-state item."),Category("Update")]
		public new event GetObjectCheckStateDelegate GetItemCheckState;
		/// <summary>
		/// Event fired when the check state of an item is changed.
		/// </summary>
		[Description("Event fired when the check state of an item is changed."),Category("Update")]
		public new event SetObjectCheckStateDelegate SetItemCheckState;

		private Dependent _depEnabled;
		private Dependent _depItems;
		private Dependent _depItemCheckState;

		private Independent _dynItemCheckState = new Independent();

		private int _updating = 0;

		/// <summary>
		/// Creates a new dependent checked list box.
		/// </summary>
		public UpdateCheckedListBox()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateCheckedListBox.Enabled", UpdateEnabled);
			_depItems = Dependent.New("UpdateCheckedListBox.Items", UpdateItems);
			_depItemCheckState = Dependent.New("UpdateCheckedListBox.ItemCheckState", UpdateItemCheckState);
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
				if ( GetItems != null )
				{
					// Use the adapter if the check state event is not defined.
					GetObjectCheckStateDelegate getItemCheckState = GetItemCheckState;
					if ( getItemCheckState == null )
						getItemCheckState = new GetObjectCheckStateDelegate( AdaptGetItemCheckState );
					SetObjectCheckStateDelegate setItemCheckState = SetItemCheckState;
					if ( setItemCheckState == null )
						setItemCheckState = new SetObjectCheckStateDelegate( AdaptSetItemCheckState );

					// Recycle the collection of items.
					ArrayList newItems = new ArrayList( base.Items.Count );
                    using (var recycleBin = new RecycleBin<CheckedListBoxItem>())
					{
                        foreach (CheckedListBoxItem item in base.Items)
                            recycleBin.AddObject(item);

						// Extract each item from the recycle bin.
						foreach ( object item in GetItems() )
						{
							newItems.Add( recycleBin.Extract(
								new CheckedListBoxItem( item, GetItemText, getItemCheckState, setItemCheckState ) ) );
						}
					}

					// Replace the items in the control.
					base.BeginUpdate();
					try
					{
						// Make sure the same tag is selected.
						CheckedListBoxItem selectedItem = (CheckedListBoxItem)base.SelectedItem;
						object selectedTag = selectedItem == null ? null : selectedItem.Tag;
						base.Items.Clear();
						base.Items.AddRange( newItems.ToArray() );
						base.SelectedIndex = IndexOfTag( selectedTag );
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

		private int IndexOfTag( object tag )
		{
			// Scan for the tag.
			int index = 0;
			foreach ( CheckedListBoxItem item in base.Items )
			{
				if ( Object.Equals( item.Tag, tag ) )
					return index;
				++index;
			}

			// Not found.
			return -1;
		}

		private CheckState AdaptGetItemCheckState( object tag )
		{
			return GetItemChecked( tag ) ? CheckState.Checked : CheckState.Unchecked;
		}

		private void AdaptSetItemCheckState( object tag, CheckState value )
		{
			SetItemChecked( tag, value != CheckState.Unchecked );
		}

		private void UpdateItemCheckState()
		{
			++_updating;
			try 
			{
				_depItems.OnGet();

				if ( GetItemChecked != null || GetItemCheckState != null )
				{
					// If the event is defined, use it to get the checked items.
					base.BeginUpdate();
					try
					{
						int index = 0;
						ArrayList items = new ArrayList( base.Items );
						foreach ( CheckedListBoxItem item in items )
						{
							CheckState checkState = item.CheckState;
							if ( base.GetItemCheckState( index ) != checkState )
								base.SetItemCheckState( index, checkState );
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
					_dynItemCheckState.OnGet();
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
			_depItemCheckState.Dispose();
			foreach ( CheckedListBoxItem item in base.Items )
				item.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depEnabled.OnGet();
			_depItems.OnGet();
			_depItemCheckState.OnGet();
		}

		/// <summary>
		/// Handle changes to an item's check state.
		/// </summary>
		/// <param name="ice">Identifies the item being checked or unchecked.</param>
		protected override void OnItemCheck(ItemCheckEventArgs ice)
		{
			// Respond to check state changes, as long as they aren't updates.
			if ( _updating == 0 )
			{
				if ( SetItemChecked != null || SetItemCheckState != null )
				{
					// Record the item check state.
					CheckedListBoxItem item = (CheckedListBoxItem)base.Items[ice.Index];
					item.CheckState =
						ice.CurrentValue == CheckState.Checked ? CheckState.Unchecked :
						ice.CurrentValue == CheckState.Unchecked ? CheckState.Indeterminate :
							CheckState.Checked;
				}
				_dynItemCheckState.OnSet();
			}

			base.OnItemCheck (ice);
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
		}

		private static object Map( object source )
		{
			return ((CheckedListBoxItem)source).Tag;
		}

		/// <summary>
		/// The collection of items in the list (read-only).
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
		/// The collection of items that are checked (read-only).
		/// </summary>
        [Browsable(false)]
        public new IList CheckedItems
		{
			get
			{
				_depItemCheckState.OnGet();
				return new UpdateControls.Forms.Util.ReadOnlyListDecorator(
					base.CheckedItems,
					new UpdateControls.Forms.Util.MapDelegate(Map));
			}
		}

		/// <summary>
		/// The collection of item indices that are checked (read-only).
		/// </summary>
        [Browsable(false)]
        public new CheckedListBox.CheckedIndexCollection CheckedIndices
		{
			get
			{
				_depItemCheckState.OnGet();
				return base.CheckedIndices;
			}
		}
	}
}
