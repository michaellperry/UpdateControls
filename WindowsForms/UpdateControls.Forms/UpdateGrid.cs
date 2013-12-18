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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using UpdateControls.Forms.Util;

namespace UpdateControls.Forms
{
	/// <summary>A grid control that automatically updates its data.</summary>
    /// <remarks>
    /// Implement the <see cref="GetColumns"/> event to return column definitions. Then implement the
    /// <see cref="GetItems"/> event to return objects to populate the rows. Implement the
    /// <see cref="GetCellValue"/> and <see cref="SetCellValue"/> events to populate the cells
    /// and respond to edits. Finally, implement <see cref="RowAdded"/> and <see cref="RowDeleted"/>
    /// to create and delete objects when the user adds and deletes rows.
    /// </remarks>
    /// <example>
    /// An update grid control has very few events. The first that you want to implement is <see cref="GetColumns"/>.
    /// You need to return a <see cref="ColumnDefinitions"/> object. Add columns using dot-chaining:
    /// <code language="C#">
    /// private UpdateControls.Forms.ColumnDefinitions itemsGrid_GetColumns()
    /// {
    ///     return new UpdateControls.Forms.ColumnDefinitions()
    ///         .Add("Name", typeof(String))
    ///         .Add("Price", typeof(decimal))
    ///         .Add("Quantity", typeof(int))
    ///         .AddReadOnly("Total", typeof(decimal));
    /// }
    /// </code>
    /// <code language="VB">
    /// Private Function itemsGrid_GetColumns() As UpdateControls.Forms.ColumnDefinitions
    /// 	Return New UpdateControls.Forms.ColumnDefinitions() _
    /// 		.Add("Name", GetType(String)) _
    /// 		.Add("Price", GetType(Decimal)) _
    /// 		.Add("Quantity", GetType(Integer)) _
    /// 		.AddReadOnly("Total", GetType(Decimal))
    /// End Function
    /// </code>
    /// Then, implement the <see cref="GetItems"/> event to return the collection of objects. These are objects
    /// of your own type, not of some special "row" type. They don't even have to implement a
    /// special interface.
    /// <code language="C#">
    /// private System.Collections.IEnumerable itemsGrid_GetItems()
    /// {
    ///     return _order.Items;
    /// }
    /// </code>
    /// <code language="VB">
    /// Private Function itemsGrid_GetItems() As System.Collections.IEnumerable
    /// 	Return _order.Items
    /// End Function
    /// </code>
    /// Fill the grid and respond to changes by implementing <see cref="GetCellValue"/> and <see cref="SetCellValue"/>.
    /// <code language="C#">
    /// private object itemsGrid_GetCellValue(object tag, int column)
    /// {
    ///     Item item = (Item)tag;
    ///     if (column == 0)
    ///         return item.Name;
    ///     else if (column == 1)
    ///         return item.Price;
    ///     else if (column == 2)
    ///         return item.Quantity;
    ///     else ///  if (column == 3)
    ///         return item.Total;
    /// }
    /// 
    /// private void itemsGrid_SetCellValue(object tag, int column, object value)
    /// {
    ///     Item item = (Item)tag;
    ///     if (column == 0)
    ///         item.Name = (String)value;
    ///     else if (column == 1)
    ///         item.Price = (decimal)value;
    ///     else if (column == 2)
    ///         item.Quantity = (int)value;
    /// }
    /// </code>
    /// <code language="VB">
    /// Private Function itemsGrid_GetCellValue(ByVal tag As Object, ByVal column As Integer) As Object
    /// 	Dim item As Item = DirectCast(tag, Item)
    /// 	If column = 0 Then
    /// 		Return item.Name
    /// 	ElseIf column = 1 Then
    /// 		Return item.Price
    /// 	ElseIf column = 2 Then
    /// 		Return item.Quantity
    /// 	ElseIf column = 3 Then
    /// 		Return item.Total
    /// 	Else
    /// 		Return Nothing
    /// 	End If
    /// End Function
    /// 
    /// Private Sub itemsGrid_SetCellValue(ByVal tag As Object, ByVal column As Integer, ByVal value As Object)
    /// 	Dim item As Item = DirectCast(tag, Item)
    /// 	If column = 0 Then
    /// 		item.Name = DirectCast(value, String)
    /// 	ElseIf column = 1 Then
    /// 		item.Price = CDec(value)
    /// 	ElseIf column = 2 Then
    /// 		item.Quantity = CInt(value)
    /// 	End If
    /// End Sub
    /// </code>
    /// Finally, to allow the user to add and delete rows, implement the <see cref="RowAdded"/> and <see cref="RowDeleted"/> events.
    /// <code language="C#">
    /// private object itemsGrid_RowAdded()
    /// {
    ///     return _order.NewItem();
    /// }
    /// 
    /// private void itemsGrid_RowDeleted(object tag)
    /// {
    ///     _order.DeleteItem((Item)tag);
    /// }
    /// </code>
    /// <code language="VB">
    /// Private Function itemsGrid_RowAdded() As Object
    /// 	Return _order.NewItem()
    /// End Function
    /// 
    /// Private Sub itemsGrid_RowDeleted(ByVal tag As Object)
    /// 	_order.DeleteItem(DirectCast(tag, Item))
    /// End Sub
    /// </code>
    /// That’s all it takes. The UpdateGrid is actually a pretty simple control. Just don’t over use it.
    /// </example>
	[Description("A grid control that automatically updates its data."),
    ToolboxBitmap(typeof(UpdateGrid), "ToolboxImages.UpdateGrid.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItems")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateGrid : DataGridView, IEnabledControl
	{
        private static string TAG_COLUMN_NAME = "@@TAG@@";

		private class DependentDataRow : IDisposable
		{
            private UpdateGrid _grid;
            private DataTable _table;
            private object _tag;
            private UpdateController _updateController;
            private DataRow _dataRow;

            private Dependent _depValue;

            public DependentDataRow(UpdateGrid grid, DataTable table, object tag, UpdateController updateController)
			{
                _grid = grid;
                _table = table;
                _tag = tag;
                _updateController = updateController;
                _depValue = Dependent.New("UpdateDataRow.Value", UpdateValue);
            }

			public void Dispose()
			{
                _depValue.Dispose();
                using (_updateController.BeginUpdating())
                {
                    if (_dataRow != null)
                    {
                        int rowIndex = _table.Rows.IndexOf(_dataRow);
                        if ( rowIndex >= 0 )
                            _table.Rows.RemoveAt(rowIndex);
                    }
                }
            }

			public DataRow DataRow
			{
				get
				{
					return _dataRow;
				}
				set
				{
                    _dataRow = value;
                    _dataRow[TAG_COLUMN_NAME] = this;
                }
			}

			public object Tag
			{
				get
				{
					return _tag;
				}
			}

			public void OnGetValue()
			{
				_depValue.OnGet();
			}

			private void UpdateValue()
			{
                using (_updateController.BeginUpdating())
                {
                    _grid._depColumns.OnGet();
                    if (_grid._columnDefinitions != null && _grid.GetCellValue != null)
                    {
                        for (int columnIndex = 0; columnIndex < _grid._columnDefinitions.Count; ++columnIndex)
                        {
                            _dataRow[columnIndex] = _grid.GetCellValue(_tag, columnIndex);
                        }
                    }
                    // Force a refresh of the view if this row has focus.
                    if (_grid.Focused && _grid.CurrentRow.Cells[TAG_COLUMN_NAME].Value == this)
                    {
                        _grid.InvalidateRow(_grid.CurrentRow.Index);
                    }
				}
			}

			public override bool Equals( object obj )
			{
				if ( obj == null )
					return false;
				if ( obj.GetType() != GetType() )
					return false;
                DependentDataRow that = (DependentDataRow)obj;
				return Object.Equals( this._tag, that._tag );
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
		[Description("Event fired to determine whether the control is enabled."),Category("Update")]
		public event GetBoolDelegate GetEnabled;
        /// <summary>Event fired to get the column definitions.</summary>
        /// <remarks>
        /// Return a new <see cref="ColumnDefinitions"/> object using dot-chaining. Within the
        /// return statement, create a new UpdateGrid.ColumnDefinitions object and call the
        /// <see cref="ColumnDefinitions.Add(string,System.Type)"/>,
        /// <see cref="ColumnDefinitions.Add(string,System.Type,System.Windows.Forms.DataGridViewCellStyle)"/>,
        /// <see cref="ColumnDefinitions.AddReadOnly(string,System.Type)"/>,
        /// and <see cref="ColumnDefinitions.AddReadOnly(string,System.Type,System.Windows.Forms.DataGridViewCellStyle)"/> methods.
        /// These methods return the UpdateGrid.ColumnDefinitions object itself, so they can be chained to gether.
        /// <para/>
        /// This event must be handled.
        /// </remarks>
        /// <example>Dot chaining UpdateGrid.ColumnDefinitions
        /// <code language="C#">
        /// private UpdateControls.Forms.ColumnDefinitions itemsGrid_GetColumns()
        /// {
        ///     return new UpdateControls.Forms.ColumnDefinitions()
        ///         .Add("Name", typeof(String))
        ///         .Add("Price", typeof(decimal))
        ///         .Add("Quantity", typeof(int))
        ///         .AddReadOnly("Total", typeof(decimal));
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Function itemsGrid_GetColumns() As UpdateControls.Forms.ColumnDefinitions
        /// 	Return New UpdateControls.Forms.ColumnDefinitions() _
        /// 		.Add("Name", GetType(String)) _
        /// 		.Add("Price", GetType(Decimal)) _
        /// 		.Add("Quantity", GetType(Integer)) _
        /// 		.AddReadOnly("Total", GetType(Decimal))
        /// End Function
        /// </code>
        /// </example>
        [Description("Event fired to get the column definitions."), Category("Update")]
        public event GetColumnDefinitionsDelegate GetColumns;
        /// <summary>Event fired to get the list of items.</summary>
        /// <remarks>
        /// The collection of objects returned from this event are not of any predefined type, nor
        /// do they implement any predefined interface. Return objects of your own type. The library
        /// will do nothing with these objects except pass them back to other events.
        /// </remarks>
        /// <example>A collection of order items.
        /// <code language="C#">
        /// private System.Collections.IEnumerable itemsGrid_GetItems()
        /// {
        ///     return _order.Items;
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Function itemsGrid_GetItems() As System.Collections.IEnumerable
        /// 	Return _order.Items
        /// End Function
        /// </code>
        /// </example>
        [Description("Event fired to get the list of items."), Category("Update")]
        public event GetCollectionDelegate GetItems;
        /// <summary>Event fired to get the value of a cell.</summary>
        /// <remarks>
        /// Cast the tag parameter to your own type as returned from <see cref="UpdateGrid.GetItems"/>. Based on
        /// the column parameter, return the attribute of that object to display in the column.
        /// </remarks>
        /// <example>Return columns from a custom data type.
        /// <code language="C#">
        /// private object itemsGrid_GetCellValue(object tag, int column)
        /// {
        ///     Item item = (Item)tag;
        ///     if (column == 0)
        ///         return item.Name;
        ///     else if (column == 1)
        ///         return item.Price;
        ///     else if (column == 2)
        ///         return item.Quantity;
        ///     else ///  if (column == 3)
        ///         return item.Total;
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Function itemsGrid_GetCellValue(ByVal tag As Object, ByVal column As Integer) As Object
        /// 	Dim item As Item = DirectCast(tag, Item)
        /// 	If column = 0 Then
        /// 		Return item.Name
        /// 	ElseIf column = 1 Then
        /// 		Return item.Price
        /// 	ElseIf column = 2 Then
        /// 		Return item.Quantity
        /// 	ElseIf column = 3 Then
        /// 		Return item.Total
        /// 	Else
        /// 		Return Nothing
        /// 	End If
        /// End Function
        /// </code>
        /// </example>
        [Description("Event fired to get the value of a cell."), Category("Update")]
        public event GetObjectColumnValueDelegate GetCellValue;
        /// <summary>Event fired when the user edits the value of a cell.</summary>
        /// <remarks>
        /// Handle this event by casting the tag parameter to your own data type, as returned from
        /// the <see cref="GetItems"/> event. Then based on the value of the column parameter, set
        /// the appropriate property of your object.
        /// </remarks>
        /// <example>Set the property of your object based on the column parameter.
        /// <code language="C#">
        /// private void itemsGrid_SetCellValue(object tag, int column, object value)
        /// {
        ///     Item item = (Item)tag;
        ///     if (column == 0)
        ///         item.Name = (String)value;
        ///     else if (column == 1)
        ///         item.Price = (decimal)value;
        ///     else if (column == 2)
        ///         item.Quantity = (int)value;
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Sub itemsGrid_SetCellValue(ByVal tag As Object, ByVal column As Integer, ByVal value As Object)
        /// 	Dim item As Item = DirectCast(tag, Item)
        /// 	If column = 0 Then
        /// 		item.Name = DirectCast(value, String)
        /// 	ElseIf column = 1 Then
        /// 		item.Price = CDec(value)
        /// 	ElseIf column = 2 Then
        /// 		item.Quantity = CInt(value)
        /// 	End If
        /// End Sub
        /// </code>
        /// </example>
        [Description("Event fired when the user edits the value of a cell."), Category("Update")]
        public event SetObjectColumnValueDelegate SetCellValue;
        /// <summary>Event fired when the user adds a row.</summary>
        /// <remarks>
        /// When the user adds a row, the update grid needs to have an object attached to it.
        /// Add an object to the end of your collection and return it from this event. The grid
        /// will behave as if it were the last object returned from <see cref="GetItems"/>.
        /// <para/>
        /// This event is called only after the first cell is entered for the new object.
        /// Immediately afterward, <see cref="SetCellValue"/> is called, followed by
        /// <see cref="GetItems"/> to ensure that the new object is indeed in the collection.
        /// </remarks>
        /// <example>Add a new object to your collection, and return that object.
        /// <code language="C#">
        /// private object itemsGrid_RowAdded()
        /// {
        ///     return _order.NewItem();
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Function itemsGrid_RowAdded() As Object
        /// 	Return _order.NewItem()
        /// End Function
        /// </code>
        /// </example>
        [Description("Event fired when the user adds a row."), Category("Update")]
        public event GetObjectDelegate RowAdded;
        /// <summary>Event fired when the user deletes a row.</summary>
        /// <remarks>
        /// When the user selects one or more rows and presses the delete key, this
        /// event is fired for each object represented by a selected row. Cast the
        /// tag parameter to your own data type and remove it from your collection.
        /// </remarks>
        /// <example>Remove the object from your collection.
        /// <code language="C#">
        /// private void itemsGrid_RowDeleted(object tag)
        /// {
        ///     _order.DeleteItem((Item)tag);
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Sub itemsGrid_RowDeleted(ByVal tag As Object)
        /// 	_order.DeleteItem(DirectCast(tag, Item))
        /// End Sub
        /// </code>
        /// </example>
        [Description("Event fired when the user deletes a row."), Category("Update")]
        public event ObjectActionDelegate RowDeleted;

		private Dependent _depEnabled;
		private Dependent _depColumns;
		private Dependent _depItems;
		private Dependent _depItemValue;
        private Independent _dynSelection = new Independent();

        private ColumnDefinitions _columnDefinitions;
        private DataTable _table;
        private List<DependentDataRow> _rows = new List<DependentDataRow>();
        private DataRow _newRow;
        private object _newTag;
        private UpdateController _updateController = new UpdateController();

		/// <summary>
		/// Creates a new dependent grid.
		/// </summary>
		public UpdateGrid()
		{
            // Create the data source.
			_table = new DataTable();
            _table.ColumnChanged += new DataColumnChangeEventHandler(OnColumnChanged);
            _table.RowDeleting += new DataRowChangeEventHandler(OnRowDeleting);

			// Create all dependent sentries.
			_depEnabled = Dependent.New("UpdateGrid.Enabled", UpdateEnabled );
			_depColumns = Dependent.New("UpdateGrid.Columns", UpdateColumns);
			_depItems = Dependent.New("UpdateGrid.Items", UpdateItems);
			_depItemValue = Dependent.New("UpdateGrid.ItemValue", UpdateItemValue);
		}

        private void OnColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (_updateController.NotUpdating && SetCellValue != null)
            {
                using (_updateController.BeginUpdating())
                {
                    DependentDataRow row = e.Row[TAG_COLUMN_NAME] as DependentDataRow;
                    object tag = null;

                    if (row != null)
                    {
                        tag = row.Tag;
                    }
                    // If no row exists yet, create one.
                    else if (RowAdded != null)
                    {
                        tag = RowAdded();
                        _newTag = tag;
                        _newRow = e.Row;
                    }
                    // Set the cell value.
                    if (tag != null)
                    {
                        SetCellValue(tag, e.Column.Ordinal, e.ProposedValue);
                    }
                }
            }
        }

        private void OnRowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (_updateController.NotUpdating && RowDeleted != null)
            {
                DependentDataRow row = e.Row[TAG_COLUMN_NAME] as DependentDataRow;

                if (row != null)
                    RowDeleted(row.Tag);
            }
        }

        private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

		private void UpdateColumns()
		{
            using (_updateController.BeginUpdating())
            {
                if (GetColumns != null)
                {
                    // Get the column definitions
                    _columnDefinitions = GetColumns();

                    // Dispose of all rows.
                    foreach (DependentDataRow row in _rows)
                        row.Dispose();
                    _rows.Clear();

                    // Apply the column definitions to the data source.
                    if (_table.Columns.Count > 0)
                        _table.Columns.Clear();
                    foreach (ColumnDefinitions.ColumnDefinition columnDefinition in _columnDefinitions.Collection)
                    {
                        DataColumn dataColumn = _table.Columns.Add(columnDefinition.name, columnDefinition.type);
                    }
                    _table.Columns.Add(TAG_COLUMN_NAME, typeof(object));
                    base.DataSource = _table;
                    base.Columns[TAG_COLUMN_NAME].Visible = false;
                    int columnIndex = 0;
                    foreach (ColumnDefinitions.ColumnDefinition columnDefinition in _columnDefinitions.Collection)
                    {
                        DataGridViewColumn viewColumn = base.Columns[columnIndex];
                        if (SetCellValue == null || columnDefinition.readOnly)
                            viewColumn.ReadOnly = true;
                        if (columnDefinition.style != null)
                            viewColumn.DefaultCellStyle = columnDefinition.style;
                        ++columnIndex;
                    }
                }
            }
		}

		private void UpdateItems()
		{
		    using (_updateController.BeginUpdating())
			{
                _depColumns.OnGet();
                if (GetItems != null)
                {
                    // Recycle the collection of items.
                    using (var recycleBin = _rows.Recycle())
                    {
                        int rowIndex = 0;
                        // Extract each item from the recycle bin.
                        foreach (object item in GetItems())
                        {
                            DependentDataRow dependentDataRow = recycleBin.Extract(
                                new DependentDataRow(this, _table, item, _updateController));
                            _rows.Add(dependentDataRow);
                            if (dependentDataRow.DataRow == null)
                            {
                                if (Object.Equals(dependentDataRow.Tag, _newTag))
                                {
                                    // The user just added this row.
                                    dependentDataRow.DataRow = _newRow;
                                    _newTag = null;
                                    _newRow = null;
                                }
                                else
                                {
                                    // Add the new row at this position.
                                    dependentDataRow.DataRow = _table.NewRow();
                                    _table.Rows.InsertAt(dependentDataRow.DataRow, rowIndex);
                                }
                            }
                            else if (_table.Rows[rowIndex] != dependentDataRow.DataRow)
                            {
                                int priorIndex = _table.Rows.IndexOf(dependentDataRow.DataRow);
                                if (priorIndex >= 0)
                                {
                                    // Save the data.
                                    object[] data = new object[_table.Columns.Count];
                                    for (int columnIndex = 0; columnIndex < _table.Columns.Count; ++columnIndex)
                                        data[columnIndex] = dependentDataRow.DataRow[columnIndex];

                                    // Move the existing row up to this position.
                                    _table.Rows.RemoveAt(priorIndex);
                                    dependentDataRow.DataRow = _table.NewRow();
                                    _table.Rows.InsertAt(dependentDataRow.DataRow, rowIndex);

                                    // Restore the data.
                                    for (int columnIndex = 0; columnIndex < _table.Columns.Count; ++columnIndex)
                                        dependentDataRow.DataRow[columnIndex] = data[columnIndex];
                                }
                            }
                            ++rowIndex;
                        }

                        // If we haven't seen the new row, then delete it.
                        if (_newTag != null)
                        {
                            _table.Rows.Remove(_newRow);
                            _newTag = null;
                            _newRow = null;
                        }
                    }
                }
			}
		}

		private void UpdateItemValue()
		{
			_depItems.OnGet();
            foreach (DependentDataRow dependentDataRow in _rows)
                dependentDataRow.OnGetValue();
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
			_depColumns.Dispose();
			_depItems.Dispose();
			_depItemValue.Dispose();
            foreach (DependentDataRow dependentDataRow in _rows)
                dependentDataRow.Dispose();
			base.OnHandleDestroyed (e);
		}

        /// <summary>When selection changes, update the <see cref="SelectedItems"/> collection.</summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(EventArgs e)
        {
            _dynSelection.OnSet();
            base.OnSelectionChanged(e);
        }

		private void Application_Idle(object sender, EventArgs e)
		{
			// Update all dependent sentries.
			_depEnabled.OnGet();
			_depColumns.OnGet();
			_depItems.OnGet();
			_depItemValue.OnGet();
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
		/// 
		/// </summary>
        [Browsable(false)]
        public new object DataSource
		{
			get
			{
				return base.DataSource;
			}
            set { }
        }

        /// <summary>The collection of items representing the selected rows.</summary>
        /// <remarks>
        /// The objects in this collection are taken from the ones returned from the <see cref="GetItems"/> event.
        /// Rows that the user has selected are represented. The entire row must be selected, not just one or all
        /// of the cells in the row. Row selection occurs when the user clicks on the row header.
        /// </remarks>
        [Browsable(false)]
        public ICollection SelectedItems
        {
            get
            {
                _depItems.OnGet();
                _dynSelection.OnGet();
                ArrayList items = new ArrayList();
                foreach (DataGridViewRow row in base.SelectedRows)
                {
                    DependentDataRow dependentRow = (DependentDataRow)row.Cells[TAG_COLUMN_NAME].Value;
                    if (dependentRow != null)
                        items.Add(dependentRow.Tag);
                }
                return items;
            }
        }
	}

    /// <summary>
    /// 
    /// </summary>
    public class ColumnDefinitions
    {
        internal struct ColumnDefinition
        {
            public string name;
            public Type type;
            public DataGridViewCellStyle style;
            public bool readOnly;
        }

        private ArrayList _columnDefinitions = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        public ColumnDefinitions()
        {
        }

        /// <summary>Add a default read-write column to the grid.</summary>
        /// <param name="name">The name to display in the column header.</param>
        /// <param name="type">The typeof the column data. For exemple typeof(int).</param>
        /// <returns>The column definition object so you can chain calls together.</returns>
        public ColumnDefinitions Add(string name, Type type)
        {
            return Add(name, type, null, false);
        }

        /// <summary>Add a read-write column to the grid with styles.</summary>
        /// <param name="name">The name to display in the column header.</param>
        /// <param name="type">The typeof the column data. For exemple typeof(int).</param>
        /// <param name="style">The style of the column. Iniialize a style object prior to making the call.</param>
        /// <returns>The column definition object so you can chain calls together.</returns>
        public ColumnDefinitions Add(string name, Type type, DataGridViewCellStyle style)
        {
            return Add(name, type, null, false);
        }

        /// <summary>Add a default read-only column to the grid.</summary>
        /// <param name="name">The name to display in the column header.</param>
        /// <param name="type">The typeof the column data. For exemple typeof(int).</param>
        /// <returns>The column definition object so you can chain calls together.</returns>
        public ColumnDefinitions AddReadOnly(string name, Type type)
        {
            return Add(name, type, null, true);
        }

        /// <summary>Add a read-only column to the grid with styles.</summary>
        /// <param name="name">The name to display in the column header.</param>
        /// <param name="type">The typeof the column data. For exemple typeof(int).</param>
        /// <param name="style">The style of the column. Iniialize a style object prior to making the call.</param>
        /// <returns>The column definition object so you can chain calls together.</returns>
        public ColumnDefinitions AddReadOnly(string name, Type type, DataGridViewCellStyle style)
        {
            return Add(name, type, null, true);
        }

        private ColumnDefinitions Add(string name, Type type, DataGridViewCellStyle style, bool readOnly)
        {
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.name = name;
            columnDefinition.type = type;
            columnDefinition.style = style;
            columnDefinition.readOnly = readOnly;
            _columnDefinitions.Add(columnDefinition);
            return this;
        }

        internal IEnumerable Collection
        {
            get { return _columnDefinitions; }
        }

        internal int Count
        {
            get { return _columnDefinitions.Count; }
        }
    }
}
