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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace UpdateControls.Forms
{
	/// <summary>
	/// A tree view that automatically updates its properties.
	/// </summary>
	[Description("A tree view that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateTreeView), "ToolboxImages.UpdateTreeView.png"),
	DefaultProperty("Name"),
	DefaultEvent("GetItems")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    public class UpdateTreeView : TreeView, IEnabledControl
	{
		private class ItemDelegates
		{
			public GetObjectStringDelegate GetItemText;
			public SetObjectStringDelegate SetItemText;
			public GetObjectIntDelegate GetItemImageIndex;
            public GetObjectIntDelegate GetSelectedItemImageIndex;
            public GetObjectCollectionDelegate GetSubItems;
            public GetObjectBoolDelegate GetHasSubItems;
		}

		private class DependentTreeNode : TreeNode, IDisposable
		{
			private ItemDelegates _itemDelegates;

			private Dependent _depText;
			private Dependent _depImageIndex;
			private Dependent _depNodes;
			private Dependent _depRecursive;

			private Independent _dynWasExpanded = Independent.New("UpdateTreeView.WasExpanded");
            private bool _wasExpanded = false;

			private int _updating = 0;

			public DependentTreeNode( object tag, ItemDelegates itemDelegates )
			{
				base.Tag = tag;
				_itemDelegates = itemDelegates;
				_depText = Dependent.New("UpdateTreeView.Text", UpdateText);
				_depImageIndex = Dependent.New("UpdateTreeView.ImageIndex", UpdateImageIndex);
				_depNodes = Dependent.New("UpdateTreeView.Nodes", UpdateNodes);
				_depRecursive = Dependent.New("UpdateTreeView.Recursive", UpdateRecursive);
			}

			public void Dispose()
			{
				_depText.Dispose();
				_depImageIndex.Dispose();
				_depNodes.Dispose();
				_depRecursive.Dispose();
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

			public void OnGetRecursive()
			{
				_depRecursive.OnGet();
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

			private void UpdateNodes()
			{
                if (_itemDelegates.GetHasSubItems != null)
                {
                    _dynWasExpanded.OnGet();
                    if (!_wasExpanded)
                    {
                        foreach (DependentTreeNode node in base.Nodes)
                            node.Dispose();
                        base.Nodes.Clear();
                        if (_itemDelegates.GetHasSubItems(base.Tag))
                            base.Nodes.Add(new DependentTreeNode(null, _itemDelegates));
                        return;
                    }
                }

                if (_itemDelegates.GetSubItems != null)
                {
                    // Recycle the collection of items.
                    Util.CollectionHelper.RecycleCollection(
                        base.Nodes,
                        _itemDelegates.GetSubItems(base.Tag).OfType<object>().Select(item =>
                            new DependentTreeNode(item, _itemDelegates)));
                }
                else
                {
                    foreach (DependentTreeNode node in base.Nodes)
                        node.Dispose();
                    base.Nodes.Clear();
                }
			}

			private void UpdateImageIndex()
			{
                if (_itemDelegates.GetItemImageIndex != null)
                {
                    base.ImageIndex = _itemDelegates.GetItemImageIndex(base.Tag);
                    if (_itemDelegates.GetSelectedItemImageIndex != null)
                        base.SelectedImageIndex = _itemDelegates.GetSelectedItemImageIndex(base.Tag);
                    else
                        base.SelectedImageIndex = base.ImageIndex;
                }
			}

			private void UpdateRecursive()
			{
                _depText.OnGet();
                _depImageIndex.OnGet();
                _depNodes.OnGet();
                _dynWasExpanded.OnGet();
                if (_wasExpanded)
                {
                    foreach (DependentTreeNode node in base.Nodes)
                    {
                        node.OnGetRecursive();
                    }
                }
			}

			public override bool Equals( object obj )
			{
				if ( obj == null )
					return false;
				if ( obj.GetType() != GetType() )
					return false;
				DependentTreeNode that = (DependentTreeNode)obj;
				return Object.Equals( base.Tag, that.Tag );
			}

			public override int GetHashCode()
			{
				return base.Tag == null ?
					0 :
					base.Tag.GetHashCode();
			}

			public void OnSetExpanded()
			{
                if (!_wasExpanded)
                {
                    _dynWasExpanded.OnSet();
                    _wasExpanded = true;
                }
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
		public event GetObjectStringDelegate GetItemText;
		/// <summary>
		/// Event fired when the user edits the label of an item.
		/// </summary>
		[Description("Event fired when the user edits the label of an item."),Category("Update")]
		public event SetObjectStringDelegate SetItemText;
		/// <summary>
		/// Event fired to determine the image index of a tree node.
		/// </summary>
		[Description("Event fired to determine the image index of a tree node."),Category("Update")]
		public event GetObjectIntDelegate GetItemImageIndex;
		/// <summary>
		/// Event fired to determine the image index of a selected tree node.
		/// </summary>
		[Description("Event fired to determine the image index of a selected tree node."),Category("Update")]
		public event GetObjectIntDelegate GetSelectedItemImageIndex;
		/// <summary>
		/// Event fired to determine if the tree node has sub items.
		/// </summary>
        [Description("Event fired to determine if the tree node has sub items."), Category("Update")]
		public event GetObjectBoolDelegate GetHasSubItems;
		/// <summary>
		/// Event fired to determine the sub items of a tree node.
		/// </summary>
		[Description("Event fired to determine the sub items of a tree node."),Category("Update")]
		public event GetObjectCollectionDelegate GetSubItems;
		/// <summary>
		/// Event fired to get the selected node in a tree view.
		/// </summary>
		[Description("Event fired to get the selected node in a tree view."),Category("Update")]
		public event GetObjectDelegate GetSelectedNode;
		/// <summary>
		/// Event fired when the user selects a node in a tree view.
		/// </summary>
		[Description("Event fired when the user selects a node in a tree view."),Category("Update")]
		public event SetObjectDelegate SetSelectedNode;

		private Dependent _depEnabled;
		private Dependent _depNodes;
		private Dependent _depRecursive;
		private Dependent _depSelectedNode;

		private Independent _dynSelectedNode = new Independent();

		private int _updating = 0;

		private ItemDelegates _itemDelegates;

		/// <summary>
		/// Creates a new dependent tree view.
		/// </summary>
		public UpdateTreeView()
		{
            // Create all dependent sentries.
			_depEnabled = new Dependent( UpdateEnabled );
			_depNodes = new Dependent( UpdateNodes );
			_depRecursive = new Dependent( UpdateRecursive );
			_depSelectedNode = new Dependent( UpdateSelectedNode );
		}

		/// <summary>
		/// </summary>
		/// <param name="tag"></param>
		public void SelectItem( object tag )
		{
			_dynSelectedNode.OnSet();
			base.SelectedNode = NodeFromTag( tag );
			if ( SetSelectedNode != null )
				SetSelectedNode( tag );
		}

		/// <summary>
		/// </summary>
		/// <param name="tag"></param>
		public void EditItem( object tag )
		{
			SelectItem( tag );

			if ( base.SelectedNode != null )
			{
				string dummy = ((DependentTreeNode)base.SelectedNode).Text;
				base.SelectedNode.BeginEdit();
			}
		}

        public void ExpandItem(object tag)
        {
            DependentTreeNode node = NodeFromTag(tag);
            if (node != null)
            {
                node.OnSetExpanded();
                node.OnGetRecursive();
                node.Expand();
            }
        }

        public void CollapseItem(object tag)
        {
            DependentTreeNode node = NodeFromTag(tag);
            if (node != null)
            {
                node.OnSetExpanded();
                node.OnGetRecursive();
                node.Collapse();
            }
        }

        public void ToggleItem(object tag)
        {
            DependentTreeNode node = NodeFromTag(tag);
            if (node != null)
            {
                node.OnSetExpanded();
                node.OnGetRecursive();
                node.Toggle();
            }
        }

        public bool IsItemExpanded(object tag)
        {
            DependentTreeNode node = NodeFromTag(tag);
            if (node != null)
                return node.IsExpanded;
            else
                return false;
        }

		private void UpdateEnabled()
		{
			// Get the property from the event.
			if ( GetEnabled != null )
				base.Enabled = GetEnabled();
		}

		private void UpdateNodes()
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
                        // Make sure the same tag is selected.
                        Util.CollectionHelper.RecycleCollection(
                            base.Nodes,
                            GetItems().OfType<object>().Select(item =>
                                new DependentTreeNode(item, _itemDelegates)));
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

		private void UpdateRecursive()
		{
			_depNodes.OnGet();
			base.BeginUpdate();
			foreach ( DependentTreeNode item in base.Nodes )
			{
				item.OnGetRecursive();
			}
			base.EndUpdate();
		}

		private void UpdateSelectedNode()
		{
			++_updating;
			try
			{
				if ( GetSelectedNode != null )
				{
					// Selection is dependent.
					base.SelectedNode = NodeFromTag( GetSelectedNode() );
				}
				else
				{
					// Selection is dynamic.
                    _depRecursive.OnGet();
                    _dynSelectedNode.OnGet();
				}
			}
			finally
			{
				--_updating;
			}
		}

		private DependentTreeNode NodeFromTag( object tag )
		{
            _depRecursive.OnGet();
			return RecursiveNodeFromTag(base.Nodes, tag);
		}

        private DependentTreeNode RecursiveNodeFromTag(TreeNodeCollection nodes, object tag)
        {
            foreach (DependentTreeNode node in nodes)
            {
                if (object.Equals(node.Tag, tag))
                    return node;
                DependentTreeNode found = RecursiveNodeFromTag(node.Nodes, tag);
                if (found != null)
                    return found;
            }
            return null;
        }

		/// <summary>
		/// Register idle-time updates for the control.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			_itemDelegates = new ItemDelegates();
			_itemDelegates.GetItemText = GetItemText;
			_itemDelegates.SetItemText = SetItemText;
			_itemDelegates.GetItemImageIndex = GetItemImageIndex;
            _itemDelegates.GetSelectedItemImageIndex = GetSelectedItemImageIndex;
			_itemDelegates.GetSubItems = GetSubItems;
            _itemDelegates.GetHasSubItems = GetHasSubItems;

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
			_depNodes.Dispose();
			_depRecursive.Dispose();
			_depSelectedNode.Dispose();

			foreach ( DependentTreeNode item in base.Nodes )
				item.Dispose();
			base.OnHandleDestroyed (e);
		}

		/// <summary>
		/// Handle changes to item selection.
		/// </summary>
		/// <param name="e">unused</param>
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			// Set the selected node.
			_dynSelectedNode.OnSet();
			if ( SetSelectedNode != null )
				SetSelectedNode( e.Node.Tag );

			base.OnAfterSelect (e);
		}

		/// <summary>
		/// Handle changes to item text.
		/// </summary>
		/// <param name="e">Specifies the modified text.</param>
		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			if ( _updating == 0 && !e.CancelEdit && e.Label != null )
			{
				DependentTreeNode item = (DependentTreeNode)e.Node;
				item.Text = e.Label;

				// The assignment was only a suggestion. See if it was honored.
				if ( item.Text != e.Label )
					e.CancelEdit = true;
			}
			else
				e.CancelEdit = true;

			base.OnAfterLabelEdit (e);
		}

		/// <summary>
		/// Handle node expansion.
		/// </summary>
		/// <param name="e">Specifies the node that has expanded.</param>
		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			// Notify the node that it has expanded.
			((DependentTreeNode)e.Node).OnSetExpanded();
			base.OnAfterExpand (e);
		}

		/// <summary>
		/// Handle node collapse.
		/// </summary>
		/// <param name="e">Specifies the node that has collapsed.</param>
		protected override void OnAfterCollapse(TreeViewEventArgs e)
		{
			// Notify the node that it has collapsed.
			((DependentTreeNode)e.Node).OnSetExpanded();
			base.OnAfterCollapse (e);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
            if (!this.Capture)
            {
                // Update all dependent sentries.
                _depEnabled.OnGet();
                _depNodes.OnGet();
                _depRecursive.OnGet();
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

		/// <summary>
		/// Collection of items in the tree view (read-only).
		/// </summary>
        [Browsable(false)]
        public new ArrayList Nodes
		{
			get
			{
				_depNodes.OnGet();
				ArrayList items = new ArrayList( base.Nodes.Count );
				foreach ( DependentTreeNode item in base.Nodes )
					items.Add( item.Tag );
				return items;
			}
		}

		/// <summary>
		/// The node that is currently selected (read-only).
		/// </summary>
        [Browsable(false)]
        public new object SelectedNode
		{
			get
			{
				_depSelectedNode.OnGet();
                if (base.SelectedNode == null)
                    return null;
                else
    				return base.SelectedNode.Tag;
			}
            set { }
        }
	}
}
