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
using System.Data;
using System.Windows.Forms;
using System.Linq;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A combo box that automatically updates its properties.
	/// </summary>
	[Description("A combo box that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateComboBox), "ToolboxImages.UpdateComboBox.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItems")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateComboBox : ComboBox, IEnabledControl, IErrorControl
	{
		private class ComboBoxItem : IDisposable
		{
			private object _tag;
			private string _text;
			private Dependent _depText;
			private GetObjectStringDelegate _getItemText;

			public ComboBoxItem( object tag, GetObjectStringDelegate getItemText )
			{
				_tag = tag;
				_getItemText = getItemText;
				_depText = Dependent.New("ComboBoxItem.Text", UpdateText );
			}

			public void Dispose()
			{
				_depText.Dispose();
			}

			public object Tag
			{
				get
				{
					return _tag;
				}
			}

			private void UpdateText()
			{
				_text = _getItemText == null ?
					_tag == null ?
						"" :
						_tag.ToString() :
					_getItemText( _tag );
				if ( _text == null )
					_text = "";
			}

			public override bool Equals( object obj )
			{
				if ( obj.GetType() != GetType() )
					return false;
				ComboBoxItem that = (ComboBoxItem)obj;
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
		/// Event fired to get the selected item.
		/// </summary>
		[Description("Event fired to get the selected item."),Category("Update")]
		public event GetObjectDelegate GetSelectedItem;
		/// <summary>
		/// Event fired when the user selects an item.
		/// </summary>
		[Description("Event fired when the user selects an item."),Category("Update")]
		public event SetObjectDelegate SetSelectedItem;

		private Dependent _depText;
		private Dependent _depEnabled;
		private Dependent _depItems;
		private Dependent _depSelectedItem;

		private Independent _dynModified = new Independent();
		private Independent _dynSelectedItem = new Independent();
        private Independent _dynError = new Independent();

		private int _updating = 0;
		private bool _modified = false;
		private int _selectedIndex = -1;
        private string _errorText = string.Empty;

		/// <summary>
		/// Creates a new dependent combo box.
		/// </summary>
		public UpdateComboBox()
		{
            // Create all dependent sentries.
			_depText = Dependent.New("UpdateComboBox.Text", UpdateText );
			_depEnabled = Dependent.New("UpdateComboBox.Enabled", UpdateEnabled);
			_depItems = Dependent.New("UpdateComboBox.Items", UpdateItems);
			_depSelectedItem = Dependent.New("UpdateComboBox.SelectedItem", UpdateSelectedItem);
		}

		private void UpdateText()
		{
			++_updating;
			try 
			{
				if ( base.DropDownStyle == ComboBoxStyle.DropDownList )
				{
					// If the control does not allow typing, then the
					// text of the selected item is it.
					_dynSelectedItem.OnGet();
					if ( _selectedIndex >= 0 )
					{
						// If an item is selected, then it controls the text.
						ComboBoxItem item = (ComboBoxItem)base.Items[ _selectedIndex ];
						item.ToString();
					}
				}
				else
				{
					// If the control allows typing, then the user and
					// GetText determine the text.
					// See if the user is typing.
					_dynModified.OnGet();
                    _dynError.OnGet();
                    if (!_modified && GetText != null && string.IsNullOrEmpty(_errorText))
					{
						// Get the text from the event.
						base.Text = GetText();
						if ( base.Focused )
							base.SelectAll();
					}
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

		private void UpdateItems()
		{
			++_updating;
			try
			{
				if ( GetItems != null )
				{
					// Replace the items in the control.
					base.BeginUpdate();
					try
					{
						if ( base.DropDownStyle == ComboBoxStyle.DropDownList )
						{
							// Make sure the same tag is selected.
							ComboBoxItem selectedItem = (ComboBoxItem)base.SelectedItem;
							object selectedTag = selectedItem == null ? null : selectedItem.Tag;
                            RecycleItems();
                            _selectedIndex = IndexOfTag(selectedTag);
							base.SelectedIndex = _selectedIndex;
						}
						else
						{
							// Make sure the same text is selected.
							string selectedText = base.Text;
                            RecycleItems();
							_selectedIndex = IndexOfText( selectedText );
							base.SelectedIndex = _selectedIndex;
						}
						if ( base.Focused )
							base.SelectAll();
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

        private void RecycleItems()
        {
            // Recycle the collection of items.
            Util.CollectionHelper.RecycleCollection(
                base.Items,
                GetItems().OfType<object>().Select(item => new ComboBoxItem(item, GetItemText)));
        }

        private int IndexOfTag(object tag)
		{
			// Scan for the tag.
			int index = 0;
			foreach ( ComboBoxItem item in base.Items )
			{
				if ( Object.Equals( item.Tag, tag ) )
					return index;
				++index;
			}

			// Not found.
			return -1;
		}

		private int IndexOfText( string text )
		{
			// Scan for the text.
			int index = 0;
			foreach ( ComboBoxItem item in base.Items )
			{
				if ( item.ToString() == text )
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

				if ( base.DropDownStyle != ComboBoxStyle.DropDownList )
				{
					// If the control allows typing, then the text
					// determines the selected item.
					_depText.OnGet();
					_selectedIndex = IndexOfText( base.Text );
				}
				else if ( GetSelectedItem != null )
				{
					// If the event is defined, use it to get the selected item.
					_selectedIndex = IndexOfTag( GetSelectedItem() );
					base.SelectedIndex = _selectedIndex;
				}
				else
				{
					_dynSelectedItem.OnGet();
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
			_depText.Dispose();
			_depEnabled.Dispose();
			_depItems.Dispose();
			_depSelectedItem.Dispose();
			foreach ( ComboBoxItem item in base.Items )
				item.Dispose();
			base.OnHandleDestroyed (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depText.OnGet();
			_depEnabled.OnGet();
			_depItems.OnGet();
			_depSelectedItem.OnGet();
		}

		/// <summary>
		/// Handle changes to the combo box text.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnTextChanged( EventArgs e )
		{
			// Respond to text changes, as long as they aren't updates.
			if ( base.DropDownStyle != ComboBoxStyle.DropDownList &&
				_updating == 0 )
			{
				// Modify the text.
				_dynModified.OnSet();
				_modified = true;
                if (SetText != null)
                    CallSetText(base.Text);
			}

			base.OnTextChanged( e );
		}

        private void CallSetText(string text)
        {
            string errorText = string.Empty;
            try
            {
                SetText(text);
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
		/// Handle changes to combo box item selection.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnSelectedIndexChanged( EventArgs e )
		{
			// Respond to selection changes, as long as they aren't updates.
			if ( _updating == 0 )
			{
				if ( base.DropDownStyle != ComboBoxStyle.DropDownList &&
					_modified )
				{
					_dynModified.OnSet();
					_modified = false;
				}

				ComboBoxItem item = (ComboBoxItem)base.SelectedItem;
				if ( SetSelectedItem != null )
					SetSelectedItem( item == null ? null : item.Tag );
				else
				{
					_dynSelectedItem.OnSet();
					_selectedIndex = base.SelectedIndex;
				}
				if ( base.DropDownStyle != ComboBoxStyle.DropDownList &&
					SetText != null )
					CallSetText( item == null ? "" : item.ToString() );
			}

			base.OnSelectedIndexChanged( e );
		}

		/// <summary>
		/// Return control of the combo box to dependency.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLeave( EventArgs e )
		{
			if ( _modified )
			{
				_dynModified.OnSet();
				_modified = false;
			}

			base.OnLeave( e );
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
		}

		private static object Map( object source )
		{
			return ((ComboBoxItem)source).Tag;
		}

		/// <summary>
		/// The items that are in the combo box (read-only).
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
		/// The item in the combo box that is selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new object SelectedItem
		{
			get
			{
				_depSelectedItem.OnGet();
				if ( _selectedIndex >= 0 )
					return ((ComboBoxItem)base.Items[_selectedIndex]).Tag;
				else
					return null;
			}
            set { }
        }

		/// <summary>
		/// The index of the currently selected item (read-only).
		/// </summary>
        [Browsable(false)]
        public new int SelectedIndex
		{
			get
			{
				_depSelectedItem.OnGet();
				return _selectedIndex;
			}
            set { }
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
