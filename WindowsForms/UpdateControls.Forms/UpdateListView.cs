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
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Forms
{
	/// <summary>A list view that automatically updates its data.</summary>
    /// <remarks>
    /// To use a list view, first set up the properties. You probably want to
    /// set View to Details, and add Columns. It is also is useful to set
    /// HideSelection to false.
    /// <para/>
    /// Now choose a data type for the items in the list. This is
    /// your own type, not a special row, and it does not implement
    /// a predefined inerface. It should probably be the data type you use
    /// for all your business logic. If it implements ToString, text will be
    /// automatically displayed. If not, don't worry; you will just have to
    /// implement one more event.
    /// <para/>
    /// The first event to implement is <see cref="GetItems"/>. Return a
    /// collection of your objects. Then if your class doesn't implement
    /// ToString (or if you want different text to appear), implement
    /// <see cref="GetItemText"/>. This text is used for the first column
    /// of the detail view; for the other columns implement <see cref="GetSubItems"/>.
    /// Return a collection of strings to display in the remaining columns.
    /// <para/>
    /// Selection in the list view typically represents UI state, not data.
    /// You can access the collection of objects that the user has selected with
    /// the <see cref="SelectedItems"/> property.
    /// <para/>
    /// But sometimes list selection represents data. You might have a collection
    /// in your data set that represents a subset of the items in the list, and you
    /// want selection to reflect this subset. For better usability, I'd recommend
    /// setting the CheckBoxes property and using the
    /// <see cref="GetItemChecked"/> and <see cref="SetItemChecked"/>
    /// events for this. But if it has to be selection, then just implement the
    /// <see cref="GetItemSelected"/> and <see cref="SetItemSelected"/> events.
    /// </remarks>
    /// <example>
    /// Return the business objects to display in the list from <see cref="GetItems"/>.
    /// <code language="C#">
    /// private System.Collections.IEnumerable personListView_GetItems()
    /// {
    ///     // Return all person objects.
    ///     return _document.People;
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Function personListView_GetItems() As System.Collections.IEnumerable
    ///		' Return all person objects.
    ///		Return _document.People
    ///	End Function
    /// </code>
    /// If the business object doesn't implement ToString, return the text to
    /// display in the first column from <see cref="GetItemText"/>. Cast the
    /// tag parameter to your own data type.
    /// <code language="C#">
    /// private string personListView_GetItemText(object tag)
    /// {
    ///     // We know that the tag is a person,
    ///     // since it was returned from GetItems.
    ///     Person person = (Person)tag;
    ///     // Display last name, first name in the first column
    ///     return person.LastFirst;
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Function personListView_GetItemText(ByVal tag As Object) As String
    ///		' We know that the tag is a person,
    ///		' since it was returned from GetItems.
    ///		Dim person As Person = DirectCast(tag, Person)
    ///		' Display last name, first name in the first column
    ///		Return person.LastFirst
    ///	End Function
    /// </code>
    /// Return the strings to display in the remaining columns from <see cref="GetSubItems"/>.
    /// The ToString method is used to convert each object to a string.
    /// <code language="C#">
    /// private System.Collections.IEnumerable personListView_GetSubItems(object tag)
    /// {
    ///     // Display age and occupation in the second and third column.
    ///     Person person = (Person)tag;
    ///     yield return person.Age;
    ///     yield return person.Occupation;
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Function personListView_GetSubItems(ByVal tag As Object) As System.Collections.IEnumerable
    ///		' Display age and occupation in the second and third column.
    ///		Dim person As Person = DirectCast(tag, Person)
    ///		Dim subItems as New List
    ///		subItems.Add(person.Age)
    ///		subItems.Add(person.Occupation)
    ///		Return subItems
    ///	End Function
    /// </code>
    /// If you have buttons that invoke operations on the selected items of the list,
    /// implement their <see cref="UpdateButton.Enabled"/> events to enable them only
    /// when some items are selected.
    /// <code language="C#">
    /// private bool ButtonEnabled()
    /// {
    ///     // The edit and delete buttons are enabled when a person is selected.
    ///     return personListView.SelectedItems.Count > 0;
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Function ButtonEnabled() As Boolean
    ///		' The edit and delete buttons are enabled when a person is selected.
    ///		Return personListView.SelectedItems.Count > 0
    ///	End Function
    /// </code>
    /// When you add an object to your collection, it will automatically appear in the
    /// list view. But you might want to select it as well.
    /// <code language="C#">
    /// private void addButton_Click(object sender, EventArgs e)
    /// {
    ///     // Create the new person object.
    ///     Person person = _document.NewPerson();
    ///     // Select the new person object.
    ///     personListView.SelectItem(person);
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Sub addButton_Click(ByVal sender As Object, ByVal e As EventArgs)
    ///		' Create the new person object.
    ///		Dim person As Person = _document.NewPerson()
    ///		' Select the new person object.
    ///		personListView.SelectItem(person)
    ///	End Sub
    /// </code>
    /// Operations that act on selected objects can get them from the
    /// <see cref="SelectedItems"/> collection.
    /// <code language="C#">
    /// private void editButton_Click(object sender, EventArgs e)
    /// {
    ///     // Open each selected person.
    ///     foreach (Person person in personListView.SelectedItems)
    ///         EditPerson(person);
    /// }
    /// 
    /// private void deleteButton_Click(object sender, EventArgs e)
    /// {
    ///     // Delete each selected person.
    ///     foreach (Person person in personListView.SelectedItems)
    ///         _document.DeletePerson(person);
    /// }
    /// </code>
    /// <code language="VB">
    ///	Private Sub editButton_Click(ByVal sender As Object, ByVal e As EventArgs)
    ///		' Open each selected person.
    ///		For Each person As Person In personListView.SelectedItems
    ///			EditPerson(person)
    ///		Next
    ///	End Sub
    /// 
    ///	Private Sub deleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
    ///		' Delete each selected person.
    ///		For Each person As Person In personListView.SelectedItems
    ///			_document.DeletePerson(person)
    ///		Next
    ///	End Sub
    /// </code>
    /// </example>
	[Description("A list view that automatically updates its properties."),
    ToolboxBitmap(typeof(UpdateListView), "ToolboxImages.UpdateListView.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItems")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateListView : ListView, IEnabledControl
	{
		private class ItemDelegates
		{
            public GetObjectObjectDelegate GetItemGroup;
			public GetObjectStringDelegate GetItemText;
			public SetObjectStringDelegate SetItemText;
			public GetObjectBoolDelegate GetItemSelected;
			public SetObjectBoolDelegate SetItemSelected;
			public GetObjectBoolDelegate GetItemChecked;
			public SetObjectBoolDelegate SetItemChecked;
			public GetObjectIntDelegate GetItemImageIndex;
			public GetObjectCollectionDelegate GetSubItems;
		}

		private class DependentListViewItem : ListViewItem, IDisposable
		{
            private IDictionary<object, ListViewGroup> _groupsByTag;
			private ItemDelegates _itemDelegates;

            private Dependent _depGroup;
			private Dependent _depText;
			private Dependent _depSelected;
			private Dependent _depChecked;
			private Dependent _depImageIndex;
			private Dependent _depSubItems;

			private Independent _dynSelected = Independent.New("UpdateListView.Selected");
			private Independent _dynChecked = Independent.New("UpdateListView.Checked");

			private bool _selected = false;
			private bool _checked = false;

			private int _updating = 0;

            public DependentListViewItem(object tag, IDictionary<object, ListViewGroup> groupsByTag, ItemDelegates itemDelegates)
			{
				base.Tag = tag;
                _groupsByTag = groupsByTag;
				_itemDelegates = itemDelegates;
                _depGroup = Dependent.New("UpdateListViewItem.Group", UpdateGroup);
				_depText = Dependent.New("UpdateListViewItem.Text", UpdateText);
				_depSelected = Dependent.New("UpdateListViewItem.Selected", UpdateSelected);
				_depChecked = Dependent.New("UpdateListViewItem.Checked", UpdateChecked);
				_depImageIndex = Dependent.New("UpdateListViewItem.ImageIndex", UpdateImageIndex);
				_depSubItems = Dependent.New("UpdateListViewItem.SubItems", UpdateSubItems);
			}

			public void Dispose()
			{
                _depGroup.Dispose();
				_depText.Dispose();
				_depSelected.Dispose();
				_depChecked.Dispose();
				_depImageIndex.Dispose();
				_depSubItems.Dispose();
			}

			public void SetSelected()
			{
				if ( _updating == 0 && _selected != base.Selected )
				{
					if ( _itemDelegates.SetItemSelected != null )
						_itemDelegates.SetItemSelected( base.Tag, base.Selected );
					_selected = base.Selected;
					_dynSelected.OnSet();
				}
			}

			/// <summary>
			/// Text displayed in the control.
			/// </summary>
			public new string Text
			{
				get
				{
					_depText.OnGet();
					return base.Text;
				}
				set
				{
					if ( _updating == 0 && base.Text != value )
					{
						if ( _itemDelegates.SetItemText != null )
							_itemDelegates.SetItemText( base.Tag, value );
					}
				}
			}

			public new int ImageIndex
			{
				get
				{
					_depImageIndex.OnGet();
					return base.ImageIndex;
				}
			}

			public new bool Selected
			{
				get
				{
					_depSelected.OnGet();
					return base.Selected;
				}
				set
				{
					if ( _updating == 0 && _selected != value )
					{
						if ( _itemDelegates.SetItemSelected != null )
							_itemDelegates.SetItemSelected( base.Tag, value );
						_selected = value;
						_dynSelected.OnSet();

						base.Selected = value;
					}
				}
			}

			public new bool Checked
			{
				get
				{
					_depChecked.OnGet();
					return base.Checked;
				}
				set
				{
					if ( _updating == 0 && _checked != value )
					{
						if ( _itemDelegates.SetItemChecked != null )
							_itemDelegates.SetItemChecked( base.Tag, value );
						_checked = value;
						_dynChecked.OnSet();

						base.Checked = value;
					}
				}
			}

			public void OnGetSubItems()
			{
				_depSubItems.OnGet();
			}

            public void OnGetGroups()
            {
                _depGroup.OnGet();
            }

            public void UpdateGroup()
            {
                ++_updating;
                try
                {
                    if (_itemDelegates.GetItemGroup != null)
                    {
                        // Look for the object among the groups.
                        object groupTag = _itemDelegates.GetItemGroup(base.Tag);
                        base.Group = _groupsByTag[groupTag];
                    }
                }
                finally
                {
                    --_updating;
                }
            }

			private void UpdateText()
			{
				++_updating;
				try
				{
					base.Text =
						_itemDelegates.GetItemText == null ?
							base.Tag == null ?
								"" :
								base.Tag.ToString() :
							_itemDelegates.GetItemText( base.Tag );
					if ( base.Text == null )
						base.Text = "";
				}
				finally
				{
					--_updating;
				}
			}

			private void UpdateSelected()
			{
				++_updating;
				try
				{
					if ( _itemDelegates.GetItemSelected != null )
						_selected = _itemDelegates.GetItemSelected( base.Tag );
					else
						_dynSelected.OnGet();
					base.Selected = _selected;
				}
				finally
				{
					--_updating;
				}
			}

			private void UpdateChecked()
			{
				++_updating;
				try
				{
					if ( _itemDelegates.GetItemChecked != null )
						_checked = _itemDelegates.GetItemChecked( base.Tag );
					else
						_dynChecked.OnGet();
					base.Checked = _checked;
				}
				finally
				{
					--_updating;
				}
			}

			private void UpdateSubItems()
			{
				if ( _itemDelegates.GetSubItems != null )
				{
					ArrayList subItems = new ArrayList();
					foreach ( object subItem in _itemDelegates.GetSubItems( base.Tag ) )
					{
						subItems.Add(
							subItem == null ? "" : subItem.ToString() );
					}
					while ( base.SubItems.Count > 1 )
						base.SubItems.RemoveAt( 1 );
					base.SubItems.AddRange( (string[])subItems.ToArray(typeof(string)) );
				}
			}

			private void UpdateImageIndex()
			{
				if ( _itemDelegates.GetItemImageIndex != null )
					base.ImageIndex = _itemDelegates.GetItemImageIndex( base.Tag );
			}

			public override bool Equals( object obj )
			{
				if ( obj == null )
					return false;
				if ( obj.GetType() != GetType() )
					return false;
				DependentListViewItem that = (DependentListViewItem)obj;
				return Object.Equals( base.Tag, that.Tag );
			}

			public override int GetHashCode()
			{
				return base.Tag == null ?
					0 :
					base.Tag.GetHashCode();
			}
		}

        private class GroupDelegates
        {
            public GetObjectStringDelegate GetGroupName;
            public GetObjectStringDelegate GetGroupHeader;
            public GetObjectHorizontalAlignmentDelegate GetGroupAlignment;
        }

        private class DependentListViewGroup : IDisposable
        {
            private object _tag;
            private GroupDelegates _groupDelegates;
            private ListView _listView;
            private ListViewGroup _listViewGroup = null;

            private Dependent _depName;
            private Dependent _depHeader;
            private Dependent _depAlignment;

            public DependentListViewGroup(object tag, GroupDelegates groupDelegates, ListView listView)
            {
                _tag = tag;
                _groupDelegates = groupDelegates;
                _listView = listView;

                _depName = Dependent.New("DependentListViewGroup.Name", UpdateName);
				_depHeader = Dependent.New("DependentListViewGroup.Header", UpdateHeader);
				_depAlignment = Dependent.New("DependentListViewGroup.Alignment", UpdateAlignment);
            }

            public void Dispose()
            {
                if (_listViewGroup != null)
                    _listView.Groups.Remove(_listViewGroup);
            }

            public object Tag
            {
                get { return _tag; }
            }

            public ListViewGroup ListViewGroup
            {
                get { return _listViewGroup; }
            }

            public void SetIndex(int index)
            {
                if (_listViewGroup == null)
                {
                    // If the group does not yet exist, create it and insert at this index.
                    _listViewGroup = new ListViewGroup();
                    _listViewGroup.Tag = this;
                    _listView.Groups.Insert(index, _listViewGroup);
                }
                else if (_listView.Groups.IndexOf(_listViewGroup) != index)
                {
                    // If the group is at the wrong index, move it.
                    _listView.Groups.Remove(_listViewGroup);
                    _listView.Groups.Insert(index, _listViewGroup);
                }
            }

            private void UpdateName()
            {
                if (_groupDelegates.GetGroupName != null)
                    _listViewGroup.Name = _groupDelegates.GetGroupName(_tag);
            }

            private void UpdateHeader()
            {
                if (_groupDelegates.GetGroupHeader != null)
                    _listViewGroup.Header = _groupDelegates.GetGroupHeader(_tag);
                else
                    _listViewGroup.Header = _tag == null ? string.Empty : _tag.ToString();
            }

            private void UpdateAlignment()
            {
                if (_groupDelegates.GetGroupAlignment != null)
                    _listViewGroup.HeaderAlignment = _groupDelegates.GetGroupAlignment(_tag);
            }

            public void OnGetProperties()
            {
                _depName.OnGet();
                _depHeader.OnGet();
                _depAlignment.OnGet();
            }

			public override bool Equals( object obj )
			{
				if ( obj == null )
					return false;
				if ( obj.GetType() != GetType() )
					return false;
                DependentListViewGroup that = (DependentListViewGroup)obj;
				return Object.Equals( _tag, that._tag );
			}

			public override int GetHashCode()
			{
				return _tag == null ?
					0 :
					_tag.GetHashCode();
			}
}

        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;
        /// <summary>Event fired to get the list of groups.</summary>
        /// <remarks>
        /// Implement this event to group items in the list view. Return objects of your own
        /// data type. The <see cref="GetItemGroup"/> event is fired to determine which group
        /// each object belogs to.
        /// <code language="C#">
        /// private System.Collections.IEnumerable personListView_GetGroups()
        /// {
        ///     return _document.Companies;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personListView_GetGroups() As System.Collections.IEnumerable
        ///		Return _document.Companies
        ///	End Function
        /// </code>
        /// If you do not implement this event, the items are not grouped.
        /// </remarks>
        [Description("Event fired to get the list of groups."), Category("Update")]
        public event GetCollectionDelegate GetGroups;
		/// <summary>Event fired to get the name of a group.</summary>
        [Description("Event fired to get the name of a group."), Category("Update")]
		public event GetObjectStringDelegate GetGroupName;
		/// <summary>Event fired to get the header of a group.</summary>
        /// <remarks>
        /// The group header is displayed above all items in the group. Cast the tag parameter
        /// to your own data type and return the string to display.
        /// <para/>
        /// If you do not implement this event, the ToString method of the object is used.
        /// </remarks>
        [Description("Event fired to get the header of a group."), Category("Update")]
		public event GetObjectStringDelegate GetGroupHeader;
		/// <summary>Event fired to get the alignment of a group.</summary>
        /// <remarks>
        /// Return HorizontalAlignment.Right, Center, or Left. Cast the tag parameter to your
        /// own data type and apply your own logic to determine which alignment to use.
        /// <para/>
        /// If you do not implement this event, HorizontalAlignment.Left is used by default.
        /// </remarks>
        [Description("Event fired to get the alignment of a group."), Category("Update")]
        public event GetObjectHorizontalAlignmentDelegate GetGroupAlignment;
        /// <summary>Event fired to get the list of items.</summary>
        /// <remarks>
        /// Return the business objects to display in the list. These are
        /// objects of your own data type. You will typically use your
        /// business object class.
        /// <code language="C#">
        /// private System.Collections.IEnumerable personListView_GetItems()
        /// {
        ///     // Return all person objects.
        ///     return _document.People;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personListView_GetItems() As System.Collections.IEnumerable
        ///		' Return all person objects.
        ///		Return _document.People
        ///	End Function
        /// </code>
        /// This event is required.
        /// </remarks>
		[Description("Event fired to get the list of items."),Category("Update")]
		public event GetCollectionDelegate GetItems;
        /// <summary>Event fired to get the group to which an item belongs.</summary>
        /// <remarks>
        /// Return the object that represents the group to which this item belogs.
        /// Cast the tag parameter to your own data type, as returned from <see cref="GetItems"/>.
        /// The object returned should be a member of the set returned by <see cref="GetGroups"/>.
        /// <code language="C#">
        /// private object personListView_GetItemGroup(object tag)
        /// {
        ///     Person person = (Person)tag;
        ///     return person.Employer;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personListView_GetItemGroup(ByVal tag As Object) As Object
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		Return person.Employer
        ///	End Function
        /// </code>
        /// If you do not implement this event, the items are not grouped.
        /// </remarks>
        [Description("Event fired to get the group to which an item belongs."), Category("Update")]
        public event GetObjectObjectDelegate GetItemGroup;
        /// <summary>Event fired to get the text associated with an item.</summary>
        /// <remarks>
        /// Return the text to display in the first column in Detail view, or
        /// as the item label in other views. The other columns of the Detail
        /// view are controlled by <see cref="GetSubItems"/>. Cast the
        /// tag parameter to your own data type.
        /// <code language="C#">
        /// private string personListView_GetItemText(object tag)
        /// {
        ///     // We know that the tag is a person,
        ///     // since it was returned from GetItems.
        ///     Person person = (Person)tag;
        ///     // Display last name, first name in the first column
        ///     return person.LastFirst;
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Function personListView_GetItemText(ByVal tag As Object) As String
        ///     ' We know that the tag is a person,
        ///     ' since it was returned from GetItems.
        ///		Dim person As Person = DirectCast(tag, Person)
        ///     ' Display last name, first name in the first column
        ///     Return person.LastFirst
        /// End Function
        /// </code>
        /// If you do not implement this event, the object's ToString method is used
        /// by default.
        /// <seealso cref="SetItemText"/>
        /// </remarks>
		[Description("Event fired to get the text associated with an item."),Category("Update")]
		public event GetObjectStringDelegate GetItemText;
		/// <summary>Event fired when the user edits the label of an item.</summary>
        /// <remarks>
        /// Cast the tag parameter to your own data type. Set the appropriate property to the
        /// value parameter that the user has entered.
        /// <code language="C#">
        /// private void personListView_SetItemText(object tag, string value)
        /// {
        ///     Person person = (Person)tag;
        ///     person.LastFirst = value;
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Sub personListView_SetItemText(ByVal tag As Object, ByVal value As String)
        ///		Dim person As Person = DirectCast(tag, Person)
        ///     person.LastFirst = value
        /// End Sub
        /// </code>
        /// <seealso cref="GetItemText"/>
        /// </remarks>
		[Description("Event fired when the user edits the label of an item."),Category("Update")]
		public event SetObjectStringDelegate SetItemText;
		/// <summary>Event fired to determine whether an item of a multi-select list view is selected.</summary>
        /// <remarks>
        /// Implement this event only if the selection represents program data. Cast the tag parameter to your
        /// own data type. Return true if the object should be selected.
        /// <para/>
        /// If you do not implement this event, selection is treated as UI state, not program data. The user
        /// is in complete control of selection, and you can use the <see cref="SelectedItems"/> property
        /// to access item selection.
        /// <seealso cref="SetItemSelected"/>
        /// </remarks>
		[Description("Event fired to determine whether an item of a multi-select list view is selected."),Category("Update")]
		public event GetObjectBoolDelegate GetItemSelected;
		/// <summary>Event fired when the user selects an item of a multi-select list view.</summary>
        /// <remarks>
        /// Implement this event only if the selection represents program data. Cast the tag parameter to your
        /// own data type and record the user's selection of deselection, as indicated by the value parameter.
        /// <para/>
        /// If you do not implement this event, selection is treated as UI state, not program data. The user
        /// is in complete control of selection, and you can use the <see cref="SelectedItems"/> property
        /// to access item selection.
        /// <seealso cref="GetItemSelected"/>
        /// </remarks>
		[Description("Event fired when the user selects an item of a multi-select list view."),Category("Update")]
		public event SetObjectBoolDelegate SetItemSelected;
		/// <summary>Event fired to determine whether an item of a check-box list view is checked.</summary>
        /// <remarks>
        /// If the CheckBoxes property is true, return true if this item is checked. Cast the tag parameter to
        /// your own data type and use your own logic to determine whether the item should be checked.
        /// <para/>
        /// If you do not implement this event and CheckBoxes is true, then all items are unchecked.
        /// <seealso cref="SetItemChecked"/>
        /// </remarks>
		[Description("Event fired to determine whether an item of a check-box list view is checked."),Category("Update")]
		public event GetObjectBoolDelegate GetItemChecked;
		/// <summary>Event fired when the user checks an item of a check-box list view.</summary>
        /// <remarks>
        /// If the CheckBoxes property is true, this event is fired when the user checks or unchecks the
        /// box next to an item. Cast the tag parameter to your own data type and use your own logic to
        /// record the user's choice.
        /// <seealso cref="GetItemChecked"/>
        /// </remarks>
		[Description("Event fired when the user checks an item of a check-box list view."),Category("Update")]
		public event SetObjectBoolDelegate SetItemChecked;
		/// <summary>Event fired to determine the image index of a list view item.</summary>
        /// <remarks>
        /// Set the SmallImageList and LargeImageList properties. Cast the tag parameter to your own
        /// data type and return the index of the image to display.
        /// </remarks>
		[Description("Event fired to determine the image index of a list view item."),Category("Update")]
		public event GetObjectIntDelegate GetItemImageIndex;
		/// <summary>Event fired to determine the sub items of a list view item.</summary>
        /// <remarks>
        /// Return the strings to display in the remaining columns. This only
        /// has an effect if the list is in Details view.
        /// <code language="C#">
        /// private System.Collections.IEnumerable personListView_GetSubItems(object tag)
        /// {
        ///     // Display age and occupation in the second and third column.
        ///     Person person = (Person)tag;
        ///     yield return person.Age;
        ///     yield return person.Occupation;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personListView_GetSubItems(ByVal tag As Object) As System.Collections.IEnumerable
        ///		' Display age and occupation in the second and third column.
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		Dim subItems as New List
        ///		subItems.Add(person.Age)
        ///		subItems.Add(person.Occupation)
        ///		Return subItems
        ///	End Function
        /// </code>
        /// If you do not implement this event, the remaining columns are not populated in
        /// Details view.
        /// </remarks>
		[Description("Event fired to determine the sub items of a list view item."),Category("Update")]
		public event GetObjectCollectionDelegate GetSubItems;

		private Dependent _depEnabled;
        private Dependent _depGroups;
        private Dependent _depGroupProperties;
		private Dependent _depItems;
        private Dependent _depItemGroups;
		private Dependent _depItemText;
		private Dependent _depItemSelected;
		private Dependent _depItemChecked;
		private Dependent _depItemImageIndex;
		private Dependent _depSubItems;

		private int _updating = 0;

        private List<DependentListViewGroup> _groups = new List<DependentListViewGroup>();

		private ItemDelegates _itemDelegates;
        private GroupDelegates _groupDelegates;
        private IDictionary<object, ListViewGroup> _groupsByTag = new Dictionary<object, ListViewGroup>();

		/// <summary>Creates a new dependent list view.</summary>
		public UpdateListView()
		{
            // Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateListView.Enabled", UpdateEnabled);
			_depGroups = Dependent.New("UpdateListView.Groups", UpdateGroups);
			_depGroupProperties = Dependent.New("UpdateListView.GroupProperties", UpdateGroupProperties);
			_depItems = Dependent.New("UpdateListView.Items", UpdateItems);
			_depItemGroups = Dependent.New("UpdateListView.ItemGroups", UpdateItemGroups);
			_depItemText = Dependent.New("UpdateListView.ItemText", UpdateItemText);
			_depItemSelected = Dependent.New("UpdateListView.ItemSelected", UpdateItemSelected);
			_depItemChecked = Dependent.New("UpdateListView.ItemChecked", UpdateItemChecked);
			_depItemImageIndex = Dependent.New("UpdateListView.ItemImageIndex", UpdateItemImageIndex);
			_depSubItems = Dependent.New("UpdateListView.SubItems", UpdateSubItems);
		}

		/// <summary>Select an item in the list view.</summary>
		/// <param name="tag">The object as retutned from GetItems that should be selected.</param>
        /// <remarks>
        /// Pass in an object that was returned from <see cref="GetItems"/>. That object will be selected.
        /// <para/>
        /// When you add an object to your collection, it will automatically appear in the
        /// list view. But you might want to select it as well.
        /// <code language="C#">
        /// private void addButton_Click(object sender, EventArgs e)
        /// {
        ///     // Create the new person object.
        ///     Person person = _document.NewPerson();
        ///     // Select the new person object.
        ///     personListView.SelectItem(person);
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Sub addButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        ///		' Create the new person object.
        ///		Dim person As Person = _document.NewPerson()
        ///		' Select the new person object.
        ///		personListView.SelectItem(person)
        ///	End Sub
        /// </code>
        /// </remarks>
        public void SelectItem(object tag)
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
				item.Selected = item.Tag == tag;
		}

		/// <summary>Open an item for editing.</summary>
		/// <param name="tag">The object as returned from GetItems that the user can edit.</param>
        /// <remarks>
        /// Pass in an object that was returned from <see cref="GetItems"/>. An edit control will open
        /// for the user to change the object name. When the user finishes editing, the
        /// <see cref="SetItemText"/> event will be fired.
        /// </remarks>
		public void EditItem( object tag )
		{
			_depItems.OnGet();
			DependentListViewItem selectedItem = null;
			foreach ( DependentListViewItem item in base.Items )
			{
				item.Selected = item.Tag == tag;
				if ( item.Tag == tag )
					selectedItem = item;
			}

			if ( selectedItem != null )
			{
				string dummy = selectedItem.Text;
				selectedItem.BeginEdit();
			}
		}

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

        private void UpdateGroups()
        {
            if (GetGroups != null)
            {
                // Recycle the groups.
                using (var bin = _groups.Recycle())
                {
                    _groups.AddRange(
                        from object g in GetGroups()
                        select bin.Extract(new DependentListViewGroup(g, _groupDelegates, this)));
                }

                // Organize the list view groups.
                _groupsByTag.Clear();
                int index = 0;
                foreach (DependentListViewGroup group in _groups)
                {
                    group.SetIndex(index++);
                    _groupsByTag[group.Tag] = group.ListViewGroup;
                }
            }
        }

        private void UpdateGroupProperties()
        {
            // Update all group properties.
            _depGroups.OnGet();
            foreach (DependentListViewGroup group in _groups)
                group.OnGetProperties();
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
                        var newItems = Util.CollectionHelper.RecycleCollection(
                            base.Items,
                            GetItems().OfType<object>().Select(item =>
                                new DependentListViewItem(item, _groupsByTag, _itemDelegates)));

                        foreach (DependentListViewItem item in newItems)
                            item.UpdateGroup();
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

        private void UpdateItemGroups()
        {
            _depGroups.OnGet();
            _depItems.OnGet();
            foreach (DependentListViewItem item in base.Items)
                item.OnGetGroups();
        }

		private void UpdateItemText()
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
			{
				string dummy = item.Text;
			}
			if ( base.Sorting != SortOrder.None )
				base.Sort();
		}

		private void UpdateItemSelected()
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
			{
				bool dummy = item.Selected;
			}
		}

		private void UpdateItemChecked()
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
			{
				bool dummy = item.Checked;
			}
		}

		private void UpdateItemImageIndex()
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
			{
				int dummy = item.ImageIndex;
			}
		}

		private void UpdateSubItems()
		{
			_depItems.OnGet();
			foreach ( DependentListViewItem item in base.Items )
			{
				item.OnGetSubItems();
			}
		}

		/// <summary>Register idle-time updates for the control.</summary>
		/// <param name="e">unused</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			_itemDelegates = new ItemDelegates();
            _itemDelegates.GetItemGroup = GetItemGroup;
			_itemDelegates.GetItemText = GetItemText;
			_itemDelegates.SetItemText = SetItemText;
			_itemDelegates.GetItemSelected = GetItemSelected;
			_itemDelegates.SetItemSelected = SetItemSelected;
			_itemDelegates.GetItemChecked = GetItemChecked;
			_itemDelegates.SetItemChecked = SetItemChecked;
			_itemDelegates.GetItemImageIndex = GetItemImageIndex;
			_itemDelegates.GetSubItems = GetSubItems;

            _groupDelegates = new GroupDelegates();
            _groupDelegates.GetGroupHeader = GetGroupHeader;
            _groupDelegates.GetGroupName = GetGroupName;
            _groupDelegates.GetGroupAlignment = GetGroupAlignment;

			// Register idle-time updates.
			Application.Idle += new EventHandler(Application_Idle);
			base.OnHandleCreated (e);
		}

		/// <summary>Unregister idle-time updates for the control.</summary>
		/// <param name="e">unused</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
            if (!RecreatingHandle)
            {
                // Unregister idle-time updates.
                Application.Idle -= new EventHandler(Application_Idle);
                _depEnabled.Dispose();
                _depGroups.Dispose();
                _depGroupProperties.Dispose();
                _depItems.Dispose();
                _depItemGroups.Dispose();
                _depItemText.Dispose();
                _depItemSelected.Dispose();
                _depItemChecked.Dispose();
                _depItemImageIndex.Dispose();
                _depSubItems.Dispose();
                foreach (DependentListViewItem item in base.Items)
                    item.Dispose();
            }
			base.OnHandleDestroyed (e);
		}

		/// <summary>Handle changes to item selection.</summary>
		/// <param name="e">unused</param>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			// Set the selected state of each item.
			foreach ( DependentListViewItem item in base.Items )
				item.SetSelected();

			base.OnSelectedIndexChanged (e);
		}

		/// <summary>Handle changes to item check state.</summary>
		/// <param name="ice">Identifies the items that are checked or unckecked.</param>
		protected override void OnItemCheck(ItemCheckEventArgs ice)
		{
			if ( _updating == 0 )
			{
				DependentListViewItem item = (DependentListViewItem)base.Items[ ice.Index ];
				item.Checked = ice.NewValue != CheckState.Unchecked;
			}

			base.OnItemCheck (ice);
		}

		/// <summary>Handle changes to item text.</summary>
		/// <param name="e">Specifies the modified text.</param>
		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			if ( _updating == 0 && !e.CancelEdit && e.Label != null )
			{
				DependentListViewItem item = (DependentListViewItem)base.Items[ e.Item ];
				item.Text = e.Label;

				// The assignment was only a suggestion. See if it was honored.
				if ( item.Text != e.Label )
					e.CancelEdit = true;
			}
			else
				e.CancelEdit = true;

			base.OnAfterLabelEdit (e);
		}


		private void Application_Idle(object sender, EventArgs e)
		{
            if (!this.Capture)
            {
                // Update all dependent sentries.
                _depEnabled.OnGet();
                _depGroups.OnGet();
                _depGroupProperties.OnGet();
                _depItems.OnGet();
                _depItemGroups.OnGet();
                _depItemText.OnGet();
                _depItemSelected.OnGet();
                _depItemChecked.OnGet();
                _depItemImageIndex.OnGet();
                _depSubItems.OnGet();
            }
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
			return ((DependentListViewItem)source).Tag;
		}

		/// <summary>
		/// Collection of items in the list view (read-only).
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

		/// <summary>Collection of items in the list view that are selected (read-only).</summary>
        /// <remarks>
        /// The SelectedItems collection contains the subset of items returned from the
        /// <see cref="GetItems"/> event that the user has selected. Use it to respond
        /// to commands that act upon the selected items.
        /// <code language="C#">
        /// private void editButton_Click(object sender, EventArgs e)
        /// {
        ///     // Open each selected person.
        ///     foreach (Person person in personListView.SelectedItems)
        ///         EditPerson(person);
        /// }
        /// 
        /// private void deleteButton_Click(object sender, EventArgs e)
        /// {
        ///     // Delete each selected person.
        ///     foreach (Person person in personListView.SelectedItems)
        ///         _document.DeletePerson(person);
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Sub editButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        ///		' Open each selected person.
        ///		For Each person As Person In personListView.SelectedItems
        ///			EditPerson(person)
        ///		Next
        ///	End Sub
        /// 
        ///	Private Sub deleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        ///		' Delete each selected person.
        ///		For Each person As Person In personListView.SelectedItems
        ///			_document.DeletePerson(person)
        ///		Next
        ///	End Sub
        /// </code>
        /// You can use the collection's Count to enable buttons only when an item is selected.
        /// <code language="C#">
        /// private bool ButtonEnabled()
        /// {
        ///     // The edit and delete buttons are enabled when a person is selected.
        ///     return personListView.SelectedItems.Count > 0;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function ButtonEnabled() As Boolean
        ///		' The edit and delete buttons are enabled when a person is selected.
        ///		Return personListView.SelectedItems.Count > 0
        ///	End Function
        /// </code>
        /// </remarks>
        [Browsable(false)]
        public new IList SelectedItems
		{
			get
			{
				_depItemSelected.OnGet();
				return new UpdateControls.Forms.Util.ReadOnlyListDecorator(
					base.SelectedItems,
					new UpdateControls.Forms.Util.MapDelegate(Map));
			}
		}

		/// <summary>
		/// Indices of the selected items in the list view (read-only).
		/// </summary>
        [Browsable(false)]
        public new ListView.SelectedIndexCollection SelectedIndices
		{
			get
			{
				_depItemSelected.OnGet();
				return base.SelectedIndices;
			}
		}

		/// <summary>
		/// Collection of items in the list view that are checked (read-only).
		/// </summary>
        [Browsable(false)]
        public new IList CheckedItems
		{
			get
			{
				_depItemChecked.OnGet();
				return new UpdateControls.Forms.Util.ReadOnlyListDecorator(
					base.CheckedItems,
					new UpdateControls.Forms.Util.MapDelegate(Map));
			}
		}

		/// <summary>
		/// Indices of items in the list view that are checked (read-only).
		/// </summary>
        [Browsable(false)]
        public new ListView.CheckedIndexCollection CheckedIndices
		{
			get
			{
				_depItemChecked.OnGet();
				return base.CheckedIndices;
			}
		}
	}
}
