/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UpdateControls;
using UpdateControls.Forms;
using UpdateControls.Themes.Renderers;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using UpdateControls.Themes.Inertia;
using WinForms = System.Windows.Forms;

namespace UpdateControls.Themes.Forms
{
    /// <summary>A tab control that updates its contents.</summary>
    [Description("A tab control that updates its contents."),
    ToolboxBitmap(typeof(ThemedTabDeck), "ToolboxImages.ThemedTabDeck.png"),
    DefaultEvent("GetItems"),
    DefaultProperty("Name")]
    public partial class ThemedTabDeck : RendererControl
    {
        #region Construction and Destruction

        /// <summary>Constructs a themed tab deck.</summary>
        public ThemedTabDeck()
        {
            _inertialRange = new InertialProperty(GetRange, delegate() { return true; });
            _depItems = new Dependent(UpdateItems);
            _depContents = new Dependent(UpdateContents);
            _depSelectedItemContent = new Dependent(UpdateSelectedItemContent);
            _depContentBounds = new Dependent(UpdateContentBounds);
            _depRestingPosition = new Dependent(UpdateRestingPosition);
            _depFloatingPosition = new Dependent(UpdateFloatingPosition);
            _depInertialPosition = new Dependent(UpdateInertialPosition);

            InitializeComponent();

            this.DoubleBuffered = true;
            this.Size = new Size(300, 200);

            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += new EventHandler(OnTimer);
            _timer.Interval = 50;

            _glideScrollRenderer = new GlideScrollRenderer(new GlideScrollRendererContext(this));
            _tabRendererContext = new TabRendererContext(this);
        }

        /// <summary>Constructs a themed tab deck within a container.</summary>
        /// <param name="container">The container of the control.</param>
        public ThemedTabDeck(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>Destroys all contents and cleans up dependencies.</summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            _timer.Stop();
            foreach (InertialRectangle r in _inertialPosition.Values)
                r.Dispose();
            foreach (ItemContent content in _contents.Values)
                content.Dispose();
            _inertialRange.Dispose();
            _depItems.Dispose();
            _depContents.Dispose();
            _depSelectedItemContent.Dispose();
            _depContentBounds.Dispose();
            _depRestingPosition.Dispose();
            _depFloatingPosition.Dispose();
            _depInertialPosition.Dispose();
            base.OnHandleDestroyed(e);
        }

        /// <summary>Brings all contents up to date.</summary>
        protected override void OnIdle()
        {
            _depSelectedItemContent.OnGet();
            _depContentBounds.OnGet();
            _depInertialPosition.OnGet();
            base.OnIdle();
        }

        #endregion

        #region Properties and Events

        private OrientationType _orientation = OrientationType.Top;
        private Theme _theme = null;
        private Theme _closeTheme = null;
        private bool _canClose = true;
        private bool _canReorder = true;
        private bool _hasImages = false;
        private bool _ownsImages = false;
        private Dynamic _dynOrientation = new Dynamic();
        private Dynamic _dynTheme = new Dynamic();
        private Dynamic _dynCanClose = new Dynamic();
        private Dynamic _dynHasImages = new Dynamic();
        private Dynamic _dynOwnsImages = new Dynamic();

        /// <summary>The position of the tabs relative to the content.</summary>
        /// <remarks>
        /// Set the position to one of the <see cref="OrientationType"/> values. These
        /// are Top, Left, Right, and Bottom. The default is Top.
        /// </remarks>
        [Category("Appearance"),
        Description("The position of the tabs relative to the content."),
        DefaultValue(OrientationType.Top)]
        public OrientationType Orientation
        {
            get { _dynOrientation.OnGet(); return _orientation; }
            set { _dynOrientation.OnSet(); _orientation = value; }
        }

        /// <summary>The set of properties that control the tab deck's appearance.</summary>
        /// <remarks>
        /// Drop a <see cref="Theme"/> component on the form with this tab deck. Set this property
        /// to that component.
        /// <para/>
        /// The font is controlled by the theme's <see cref="P:Theme.HeaderFont"/> property. The appearance
        /// of the tabs is controlled by <see cref="P:Theme.SolidRegular"/>
        /// <para/>
        /// See also:
        /// <seealso cref="CloseTheme"/>
        /// </remarks>
        [Category("Appearance"),
        Description("The set of properties that control the tab deck's appearance.")]
        public Theme Theme
        {
            get { _dynTheme.OnGet(); return _theme; }
            set { _dynTheme.OnSet(); _theme = value; }
        }

        /// <summary>The theme of the close button.</summary>
        /// <remarks>
        /// Drop a <see cref="Theme"/> component on the form with this tab deck. Set this property
        /// to that component.
        /// <para/>
        /// The appearance is controlled by the theme's <see cref="P:Theme.SolidRegular"/>, <see cref="P:Theme.SolidHover"/>,
        /// and <see cref="P:Theme.SolidPressed"/> properties.
        /// <para/>
        /// See also:
        /// <seealso cref="Theme"/>
        /// <seealso cref="CanClose"/>
        /// </remarks>
        [Category("Appearance"),
        Description("The theme of the close button.")]
        public Theme CloseTheme
        {
            get { _dynTheme.OnGet(); return _closeTheme; }
            set { _dynTheme.OnSet(); _closeTheme = value; }
        }

        /// <summary>True if the user can close the tabs.</summary>
        /// <remarks>
        /// Set this property to true if the close button should appear within the tabs. When
        /// the close button is clicked, the <see cref="CloseTab"/> event is fired. The user may
        /// also hit Ctrl+F4 to close the current tab.
        /// <para/>
        /// Set this property to false to hide the close buttons. This also disables Ctrl+F4. The
        /// CloseTab event will not be fired. Close buttons cannot be individually enabled.
        /// <para/>
        /// Close buttons are visible by default.
        /// </remarks>
        [Category("Behavior"),
        Description("True if the user can close the tabs."),
        DefaultValue(true)]
        public bool CanClose
        {
            get { _dynCanClose.OnGet(); return _canClose; }
            set { _dynCanClose.OnSet(); _canClose = value; }
        }

        /// <summary>True if the user can change the order of the tabs.</summary>
        /// <remarks>
        /// Set this property to true to allow the user to rearrange the tabs. When the user
        /// drags a tab, the others move out of its way to indicate the drop point.
        /// <para/>
        /// Set this property to false to disallow rearranging. Dragging a tab will have
        /// no effect.
        /// <para/>
        /// Reordering is enabled by default.
        /// </remarks>
        [Category("Behavior"),
        Description("True if the user can change the order of the tabs."),
        DefaultValue(true)]
        public bool CanReorder
        {
            get { return _canReorder; }
            set { _canReorder = value; }
        }

        /// <summary>True if the tabs have images</summary>
        /// <remarks>
        /// Set this property to true to reserve space within each tab for an image. The
        /// image is placed to the right of the text. The
        /// <see cref="GetItemImage"/> event is fired for each tab to retrieve the image.
        /// If a tab has no image, space is still reserved for consistency with the other
        /// tabs.
        /// <para/>
        /// Set this property to false to hide tab images. No space is reserved on any tab. The
        /// GetTabImage event is not fired.
        /// <para/>
        /// Tab images are disabled by default.
        /// </remarks>
        [Category("Appearance"),
        Description("True if the tabs have images."),
        DefaultValue(false)]
        public bool HasImages
        {
            get { _dynHasImages.OnGet(); return _hasImages; }
            set { _dynHasImages.OnSet(); _hasImages = value; }
        }

        /// <summary>True if the control owns the tab images</summary>
        /// <remarks>
        /// Set this property to true if you generate images manually within <see cref="GetItemImage"/>.
        /// The control will dispose of those images when they are no longer needed.
        /// <para/>
        /// Set this property to false if you return images managed by some other mechanism.
        /// For example, if you return images from an image list, this property shoul be false.
        /// The control will not dispose those images.
        /// <para/>
        /// Tab images are not owned by default.
        /// </remarks>
        [Category("Appearance"),
        Description("True if the control owns the tab images."),
        DefaultValue(false)]
        public bool OwnsImages
        {
            get { _dynOwnsImages.OnGet(); return _ownsImages; }
            set { _dynOwnsImages.OnSet(); _ownsImages = value; }
        }

        /// <summary>Returns the items to display in the tabs.</summary>
        /// <remarks>
        /// Return a collection of objects that represent the contents of the tabs. These
        /// are objects of your own data type. The objects
        /// in this collection will be sent back into the <see cref="GetItemText"/>,
        /// <see cref="GetItemImage"/>, and <see cref="CreateContent"/> events. The order
        /// of the objects in the collection determines the order of the tabs.
        /// <para/>
        /// This is a required event.
        /// </remarks>
        /// <example>Return a collection of person objects. One tab will appear for each person
        /// in the collection. If this is a subset of a larger collection, cull the subset to
        /// get rid of any objects that have been removed.
        /// <code language="C#">
        /// private System.Collections.IEnumerable personTabDeck_GetItems()
        /// {
        ///     // Remove all open people that have been deleted from the document.
        ///     _openPeople.RemoveAll(delegate(Person person) { return !_document.Exists(person); });
        ///     // Display all open person objects in the tabs.
        ///     _dynOpenPeople.OnGet();
        ///     return _openPeople;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personTabDeck_GetItems() As System.Collections.IEnumerable
        ///		' Remove all open people that have been deleted from the document.
        ///		_openPeople.RemoveAll(AddressOf PersonDeleted)
        ///		' Display all open person objects in the tabs.
        ///		_dynOpenPeople.OnGet()
        ///		Return _openPeople
        ///	End Function
        /// 
        ///	Private Sub PersonDeleted(ByVal person As Person)
        ///		Return Not _document.Exists(person)
        ///	End Sub
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Returns the items to display in the tabs.")]
        public event GetCollectionDelegate GetItems;

        /// <summary>Returns the text of an item to display in its tab.</summary>
        /// <remarks>
        /// The tag parameter is one member of the collection returned from the
        /// <see cref="GetItems"/> event. Return the text to display in the tab
        /// representing this object.
        /// <para/>
        /// This is an optional event. If it is not handled, the ToString method
        /// of the object is used instead.
        /// </remarks>
        /// <example>Return the name of a person object.
        /// <code language="C#">
        /// private string personTabDeck_GetItemText(object tag)
        /// {
        ///     return ((Person)tag).LastFirst;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personTabDeck_GetItemText(ByVal tag As Object) As String
        ///		Return DirectCast(tag, Person).LastFirst
        ///	End Function
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Returns the text of an item to display in its tab.")]
        public event GetObjectStringDelegate GetItemText;

        /// <summary>Returns the image of an item to display in its tab.</summary>
        /// <remarks>
        /// The tag parameter is one member of the collection returned from the
        /// <see cref="GetItems"/> event. Return the image to display in the tab
        /// representing this object.
        /// <para/>
        /// The control does not destroy the image that this event returns. You
        /// may pull the image from an image list, which manages the resources
        /// for you. Or you may generate the image yourself and manage the resoruces
        /// manually.
        /// <para/>
        /// If you return null, then no image will be displayed. However, space
        /// will still be reserved for the image.
        /// </remarks>
        /// <example>You can return an image from an image list.
        /// <code language="C#">
        /// private Image personTabDeck_GetItemImage(object tag)
        /// {
        ///     Person person = (Person)tag;
        ///     return occupationImageList.Images[person.Occupation.ToString()];
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personTabDeck_GetItemImage(ByVal tag As Object) As Image
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		Return occupationImageList.Images(person.Occupation.ToString())
        ///	End Function
        /// </code>
        /// If you draw the image manually, you must set the <see cref="OwnsImages"/> property.
        /// <code language="C#">
        /// private Image personTabDeck_GetItemImage(object tag)
        /// {
        ///     Person person = (Person)tag;
        ///     Image image = new Bitmap(50, 50);
        ///     using (Graphics g = Graphics.FromImage(image))
        ///     {
        ///         DrawImage(person, g);
        ///     }
        ///     return image;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personTabDeck_GetItemImage(ByVal tag As Object) As Image
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		Dim image As Image = New Bitmap(50, 50)
        ///		Using g As Graphics = Graphics.FromImage(image)
        ///			DrawImage(person, g)
        ///		End Using
        ///		Return image
        ///	End Function
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Returns the image of an item to display in its tab.")]
        public event GetObjectImageDelegate GetItemImage;

        /// <summary>Creates a control to display as the content of a tab</summary>
        /// <remarks>
        /// Create a user control to dislay within the body of the tab control. Create an instance
        /// of this control, initialize it with the tag parameter, and return it from this event. The
        /// control will be destroyed when it is no longer needed.
        /// <para/>
        /// This event is required.
        /// </remarks>
        /// <example>Return a new user control for the object.
        /// <code language="C#">
        /// private Control personTabDeck_CreateContent(object tag)
        /// {
        ///     Person person = (Person)tag;
        ///     PersonForm personForm = new PersonForm();
        ///     personForm.Person = (Person)tag;
        ///     return personForm;
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Function personTabDeck_CreateContent(ByVal tag As Object) As Control
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		Dim personForm As New PersonForm()
        ///		personForm.Person = DirectCast(tag, Person)
        ///		Return personForm
        ///	End Function
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Creates a control to display as the content of a tab.")]
        public event CreateControlDelegate CreateContent;

        /// <summary>Moves a tab to a new position</summary>
        /// <remarks>
        /// This event is called whenever the user rearanges tabs. Move the object given by the
        /// tag parameter to the specified position in the collection that you returned from
        /// <see cref="GetItems"/>. All items between the old and new position should shift to make room.
        /// <para/>
        /// If the <see cref="CanReorder"/> property is true, this event is required.
        /// </remarks>
        /// <example>Move a person to a new position in the list.
        /// <code language="C#">
        /// private void personTabDeck_SetTabPosition(object tag, int value)
        /// {
        ///     Person person = (Person)tag;
        ///     _dynOpenPeople.OnSet();
        ///     _openPeople.Remove(person);
        ///     _openPeople.Insert(value, person);
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Sub personTabDeck_SetTabPosition(ByVal tag As Object, ByVal value As Integer)
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		_dynOpenPeople.OnSet()
        ///		_openPeople.Remove(person)
        ///		_openPeople.Insert(value, person)
        ///	End Sub
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Moves a tab to a new position.")]
        public event SetObjectIntegerDelegate SetTabPosition;

        /// <summary>Fired when the user presses a tab close button</summary>
        /// <remarks>
        /// Remove the object given by the tag parameter from the collection that you returned from <see cref="GetItems"/>.
        /// <para/>
        /// If the <see cref="CanClose"/> property is true, this event is required.
        /// </remarks>
        /// <example>Remove the person object from the open people collection.
        /// <code language="C#">
        /// private void personTabDeck_CloseTab(object tag)
        /// {
        ///     Person person = (Person)tag;
        ///     _dynOpenPeople.OnSet();
        ///     _openPeople.Remove(person);
        /// }
        /// </code>
        /// <code language="VB">
        ///	Private Sub personTabDeck_CloseTab(ByVal tag As Object)
        ///		Dim person As Person = DirectCast(tag, Person)
        ///		_dynOpenPeople.OnSet()
        ///		_openPeople.Remove(person)
        ///	End Sub
        /// </code>
        /// </example>
        [Category("Update"),
        Description("Fired when the user presses a tab close button.")]
        public event ObjectActionDelegate CloseTab;

        #endregion

        #region Keyboard Commands

        private const int WM_KEYDOWN = 0x100;
        /// <summary>Handles Ctrl+Tab, Ctrl+Shift+Tab, and Ctrl+F4</summary>
        /// <param name="msg">The windows message</param>
        /// <param name="keyData">The key that was pressed</param>
        /// <returns>True if the message was handled.</returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (msg.Msg == WM_KEYDOWN)
            {
                if (msg.WParam == (IntPtr)WinForms.Keys.Tab)
                {
                    if ((ModifierKeys & WinForms.Keys.Modifiers) == (WinForms.Keys.Control | WinForms.Keys.Shift))
                    {
                        // Ctrl + Shift + Tab: Go back.
                        _depItems.OnGet();
                        int index = _items.IndexOf(_selectedItem);
                        if (index > 0)
                            --index;
                        else
                            index = _items.Count - 1;

                        if (index >= 0 && index < _items.Count)
                            SelectItem(_items[index]);
                        return true;
                    }
                    else if ((ModifierKeys & WinForms.Keys.Modifiers) == WinForms.Keys.Control)
                    {
                        // Ctrl + Tab: Go forward.
                        _depItems.OnGet();
                        int index = _items.IndexOf(_selectedItem);
                        if (index >= 0 && index < _items.Count - 1)
                            ++index;
                        else
                            index = 0;

                        if (index >= 0 && index < _items.Count)
                            SelectItem(_items[index]);
                        return true;
                    }
                }
                else if (msg.WParam == (IntPtr)WinForms.Keys.F4)
                {
                    if ((ModifierKeys & WinForms.Keys.Modifiers) == WinForms.Keys.Control)
                    {
                        // Ctrl + F4: Close the current tab.
                        if (CanClose && _selectedItem != null)
                        {
                            OnClickClose(_selectedItem);
                        }
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Business Layer

        #region ItemContent

        private class ItemContent : IDisposable
        {
            private ThemedTabDeck _owner;
            private object _item;
            private System.Windows.Forms.Control _content = null;
            private Dependent _depContent;

            public ItemContent(ThemedTabDeck owner, object item)
            {
                _owner = owner;
                _item = item;
                _depContent = new Dependent(UpdateContent);
            }

            private void UpdateContent()
            {
                // Destroy the previous control.
                if (_content != null)
                    _content.Dispose();

                // Create a new control.
                if (_owner.CreateContent != null)
                    _content = _owner.CreateContent(_item);
            }

            public System.Windows.Forms.Control Content
            {
                get { _depContent.OnGet(); return _content; }
            }

            public void EnsureHidden()
            {
                if (_content != null)
                    _content.Visible = false;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                if (obj == null)
                    return false;
                if (obj.GetType() != this.GetType())
                    return false;
                ItemContent that = (ItemContent)obj;
 	            return Object.Equals(this._item, that._item);
            }

            public override int  GetHashCode()
            {
 	            return _item == null ? 0 : _item.GetHashCode();
            }

            public void Dispose()
            {
                if (_content != null)
                    _content.Dispose();
                _depContent.Dispose();
            }
        }

        #endregion

        private ArrayList _items = new ArrayList();
        private Dictionary<object, ItemContent> _contents = new Dictionary<object,ItemContent>();
        private object _selectedItem = null;
        private ItemContent _selectedItemContent;
        private Rectangle _contentBounds;
        private Dependent _depItems;
        private Dependent _depContents;
        private Dynamic _dynSelectedItem = new Dynamic();
        private Dependent _depSelectedItemContent;
        private Dynamic _dynSize = new Dynamic();
        private Dependent _depContentBounds;

        private bool ChangeSelectedItem(object item)
        {
            _depItems.OnGet();
            if (!Object.Equals(item, _selectedItem) && _items.Contains(item))
            {
                _dynSelectedItem.OnSet();
                _selectedItem = item;
                return true;
            }
            return false;
        }

        private void UpdateItems()
        {
            // Get the list of items using the event.
            _items.Clear();
            if (GetItems != null)
                foreach (object item in GetItems())
                    _items.Add(item);
            else
                _items = new ArrayList(new string[] { "One", "Two", "Three" });

            // Select the first available tab.
            if (_selectedItem == null || !_items.Contains(_selectedItem))
            {
                if (_items.Count > 0)
                    SelectItem(_items[0]);
                else
                    _selectedItem = null;
            }
        }

        private void UpdateContents()
        {
            // Create a content holder for each item.
            _depItems.OnGet();
            using (RecycleBin<ItemContent> bin = new RecycleBin<ItemContent>(_contents.Values))
            {
                _contents.Clear();
                foreach (object item in _items)
                    _contents.Add(item, bin.Extract(new ItemContent(this, item)));
            }
        }

        private void UpdateSelectedItemContent()
        {
            // Get the content of the selected item.
            _depContents.OnGet();
            _dynSelectedItem.OnGet();
            if (_selectedItem == null || !_contents.TryGetValue(_selectedItem, out _selectedItemContent))
                _selectedItemContent = null;

            // Ensure that the selected item's content is visible, and that other content is not.
            foreach (ItemContent content in _contents.Values)
            {
                if (!Object.Equals(_selectedItemContent, content))
                    content.EnsureHidden();
            }
            if (_selectedItemContent != null)
            {
                System.Windows.Forms.Control content = _selectedItemContent.Content;
                if (content != null)
                {
                    content.Parent = this;
                    content.Bounds = _contentBounds;
                    content.Visible = true;
                    content.Focus();
                }
            }
        }

        private void UpdateContentBounds()
        {
            // Get the control bounds.
            _dynSize.OnGet();
            _contentBounds = this.ClientRectangle;

            // Reserve space for the tabs.
            int height = MeasureItemHeight();
            OrientationType orientation = Orientation;
            if (orientation == OrientationType.Top)
            {
                _contentBounds.Height -= height;
                _contentBounds.Y += height;
            }
            else if (orientation == OrientationType.Bottom)
            {
                _contentBounds.Height -= height;
            }
            else if (orientation == OrientationType.Left)
            {
                _contentBounds.Width -= height;
                _contentBounds.X += height;
            }
            else /*if (orientation == OrientationType.Right)*/
            {
                _contentBounds.Width -= height;
            }

            // Reposition the selected item content.
            if (_selectedItemContent != null)
            {
                System.Windows.Forms.Control content = _selectedItemContent.Content;
                if (content != null)
                    content.Bounds = _contentBounds;
            }
        }

        /// <summary>Resizes the contents of the tabs when the tab control is resized</summary>
        /// <param name="e">The new size of teh control</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            _dynSize.OnSet();
            //_depContentBounds.OnGet();
            //_depInertialPosition.OnGet();
            base.OnSizeChanged(e);
        }

        #endregion

        #region Interaction Layer

        #region InertialRectangle
        // Keep track of the inertia of each tab.
        private class InertialRectangle : IDisposable
        {
            private ThemedTabDeck _owner;
            private object _tag;
            private Inertia.InertialProperty _location;

            public InertialRectangle(ThemedTabDeck owner, object tag)
            {
                _owner = owner;
                _tag = tag;
                _location = new Inertia.InertialProperty(GetTargetValue, GetHasInertia);
            }

            private float GetTargetValue()
            {
                // Move toward the floating rectangle.
                _owner._depFloatingPosition.OnGet();
                if (_owner._orientation == OrientationType.Top || _owner._orientation == OrientationType.Bottom)
                    return _owner._floatingPosition[_tag].X;
                else
                    return _owner._floatingPosition[_tag].Y;
            }

            private bool GetHasInertia()
            {
                // The tab has inertia when it is not dragging.
                _owner._dynDrag.OnGet();
                return !Object.Equals(_tag, _owner._dragItem);
            }

            public bool OnTimer(long ticks)
            {
                return _location.OnTimer(ticks);
            }

            public Rectangle Position
            {
                get
                {
                    // Set the floating rectangle location to the inertial property.
                    _owner._depFloatingPosition.OnGet();
                    Rectangle r = _owner._floatingPosition[_tag];
                    if (_owner._orientation == OrientationType.Top || _owner._orientation == OrientationType.Bottom)
                        r.X = (int)Math.Round(_location.Value);
                    else
                        r.Y = (int)Math.Round(_location.Value);
                    return r;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                if (obj == null)
                    return false;
                if (obj.GetType() != this.GetType())
                    return false;
                InertialRectangle that = (InertialRectangle)obj;
                return Object.Equals(this._tag, that._tag);
            }

            public override int GetHashCode()
            {
                return _tag == null ? 0 : _tag.GetHashCode();
            }

            #region IDisposable Members

            public void Dispose()
            {
                _location.Dispose();
            }

            #endregion
        }
        #endregion

        // Dynamic
        private object _dragItem = null;
        private Point _dragStart;
        private Point _dragPosition;
        private Dynamic _dynDrag = new Dynamic();

        // Dependent
        private int _range;
        private InertialProperty _inertialRange;
        private Dictionary<object, Rectangle> _restingPosition = new Dictionary<object, Rectangle>();
        private Dictionary<object, Rectangle> _floatingPosition = new Dictionary<object, Rectangle>();
        private Dictionary<object, InertialRectangle> _inertialPosition = new Dictionary<object, InertialRectangle>();
        private int _dropIndex = 0;
        private Dependent _depRestingPosition;
        private Dependent _depFloatingPosition;
        private Dependent _depInertialPosition;

        private System.Windows.Forms.Timer _timer;

        /// <summary>Returns the object represented by the selected tab</summary>
        /// <returns>A member of the collection returned from <see cref="GetItems"/> that is represented
        /// by the currently selected tab.</returns>
        public object GetSelectedItem()
        {
            _dynSelectedItem.OnGet();
            _depItems.OnGet();
            return _selectedItem;
        }

        /// <summary>Selects the tab that represents the given item</summary>
        /// <param name="item">The item of the tab to select</param>
        public void SelectItem(object item)
        {
            bool changed = ChangeSelectedItem(item);

            if (changed)
            {
                // Ensure that the selected item is visible.
                _depRestingPosition.OnGet();
                Rectangle restingPosition;
                if (_restingPosition.TryGetValue(_selectedItem, out restingPosition))
                {
                    _glideScrollRenderer.EnsureVisible(restingPosition);
                }
            }
        }

        private void DragItem(object item, Point start, Point position)
        {
            if (_canReorder)
            {
                _dynDrag.OnSet();
                _dragItem = item;
                _dragStart = start;
                _dragPosition = position;

                SelectItem(item);
            }
        }

        private void EndDrag()
        {
            // Notify the application that the tab order has changed.
            if (_dragItem != null && SetTabPosition != null)
            {
                _depFloatingPosition.OnGet();
                if (!Object.Equals(_items[_dropIndex], _dragItem))
                    SetTabPosition(_dragItem, _dropIndex);
            }

            // Get out of drag mode.
            if (_dragItem != null)
            {
                _dynDrag.OnSet();
                _dragItem = null;
            }
        }

        private void UpdateRestingPosition()
        {
            // Allocate a tab position for each item.
            _restingPosition.Clear();

            OrientationType orientation = Orientation;
            int height = MeasureItemHeight();
            int spacing = height / 4;
            int position = spacing;

            bool canClose = CanClose;
            bool hasImages = HasImages;

            _depItems.OnGet();
            foreach (object item in _items)
            {
                // Measure each item at rest.
                int size = MeasureItemWidth(item);

                // Add some room for the close button.
                if (canClose)
                    size += height;

                // Add some room for the embedded image.
                if (hasImages)
                    size += height;

                // Position the item at rest here.
                _restingPosition.Add(item, GetBounds(orientation, position, height, spacing, size));
                position += size + spacing;
            }

            _range = position;
        }

        private float GetRange()
        {
            _depRestingPosition.OnGet();
            return (float)_range;
        }

        private void UpdateFloatingPosition()
        {
            _dynDrag.OnGet();
            _depRestingPosition.OnGet();
            if (_dragItem == null)
            {
                // We are not dragging. Use the resting position.
                _floatingPosition = new Dictionary<object,Rectangle>(_restingPosition);
            }
            else
            {
                _floatingPosition.Clear();

                // We are dragging. Get the center position of the item being dragged.
                OrientationType orientation = Orientation;
                Rectangle dragBounds = _restingPosition[_dragItem];
                int dragPosition;
                if (orientation == OrientationType.Left || orientation == OrientationType.Right)
                {
                    dragBounds.Y += _dragPosition.Y - _dragStart.Y;
                    dragPosition = dragBounds.Top + dragBounds.Height / 2;
                }
                else
                {
                    dragBounds.X += _dragPosition.X - _dragStart.X;
                    dragPosition = dragBounds.Left + dragBounds.Width / 2;
                }
                _floatingPosition.Add(_dragItem, dragBounds);

                // We haven't found the position of the dragging item, yet.
                bool dragging = true;

                // Allocate a tab position for each item.
                _depItems.OnGet();
                int height = GetItemHeight(orientation, dragBounds);
                int spacing = height / 4;
                int dragSize = GetItemWidth(orientation, dragBounds) - 2 * spacing;
                int position = spacing;
                int index = 0;
                foreach (object item in _items)
                {
                    if (!Object.Equals(item, _dragItem))
                    {
                        // Measure each item at rest.
                        int size = GetItemWidth(orientation, _restingPosition[item]) - 2 * spacing;

                        // Determine whether the dragging item belongs here.
                        if (dragging && dragPosition < position + (dragSize + size + spacing) / 2)
                        {
                            // Make room for the dragging item.
                            _dropIndex = index;
                            position += dragSize + spacing;
                            dragging = false;
                        }

                        // Position the item at rest here.
                        _floatingPosition.Add(item, GetBounds(orientation, position, height, spacing, size));
                        position += size + spacing;
                        ++index;
                    }
                }

                // If we didn't find the drop index, put it at the end.
                if (dragging)
                {
                    _dropIndex = index;
                }
            }

            // Start the inertia timer.
            _timer.Start();
        }

        private void UpdateInertialPosition()
        {
            _depItems.OnGet();
            using (RecycleBin<InertialRectangle> bin = new RecycleBin<InertialRectangle>(_inertialPosition.Values))
            {
                _inertialPosition.Clear();
                foreach (object tag in _items)
                    _inertialPosition[tag] = bin.Extract(new InertialRectangle(this, tag));
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            // Notify all inertial rectangles of the time.
            long ticks = Inertia.InertialProperty.GetTickCount();
            _depInertialPosition.OnGet();
            bool changed = _inertialRange.OnTimer(ticks);
            foreach (InertialRectangle r in _inertialPosition.Values)
                changed = r.OnTimer(ticks) || changed;

            // Stop the timer if nothing is changing.
            if (!changed)
                _timer.Stop();
        }

        private int GetItemWidth(OrientationType orientation, Rectangle rectangle)
        {
            if (orientation == OrientationType.Left || orientation == OrientationType.Right)
                return rectangle.Height;
            else
                return rectangle.Width;
        }

        private int GetItemHeight(OrientationType orientation, Rectangle rectangle)
        {
            if (orientation == OrientationType.Left || orientation == OrientationType.Right)
                return rectangle.Width;
            else
                return rectangle.Height;
        }

        private Rectangle GetBounds(OrientationType orientation, int position, int height, int spacing, int size)
        {
            Rectangle bounds;
            if (orientation == OrientationType.Top)
                bounds = new Rectangle(position - spacing, 0, size + 2 * spacing, height);
            else if (orientation == OrientationType.Bottom)
                bounds = new Rectangle(position - spacing, Height - height, size + 2 * spacing, height);
            else if (orientation == OrientationType.Left)
                bounds = new Rectangle(0, position - spacing, height, size + 2 * spacing);
            else /*if (orientation == OrientationType.Right)*/
                bounds = new Rectangle(Width - height, position - spacing, height, size + 2 * spacing);
            return bounds;
        }

        private int MeasureItemWidth(object item)
        {
            return System.Windows.Forms.TextRenderer.MeasureText(GetText(item), DefaultTheme.FontHeader(Theme)).Width;
        }

        private int MeasureItemHeight()
        {
            return DefaultTheme.FontHeader(Theme).Height;
        }

        #endregion

        #region GlideScrollRenderer

        private class GlideScrollRendererContext : GlideScrollRenderer.Context
        {
            private ThemedTabDeck _owner;

            public GlideScrollRendererContext(ThemedTabDeck owner)
            {
                _owner = owner;
            }

            public Rectangle Bounds
            {
                get { return _owner.TabBounds; }
            }

            public int Range
            {
                get { return _owner.TabRange; }
            }

            public bool Horizontal
            {
                get { OrientationType orientation = _owner.Orientation; return orientation == OrientationType.Top || orientation == OrientationType.Bottom; }
            }

            public IEnumerable<Renderer> GetChildRenderers()
            {
                return _owner.GetTabRenderers();
            }

            public void Invalidate(Rectangle bounds)
            {
                _owner.Invalidate(bounds);
            }

            public IntPtr Handle
            {
                get { return _owner.Handle; }
            }
        }

        private GlideScrollRenderer _glideScrollRenderer;

        private Rectangle TabBounds
        {
            get
            {
                Rectangle bounds;
                OrientationType orientation = Orientation;
                int height = MeasureItemHeight();
                _dynSize.OnGet();
                if (orientation == OrientationType.Top)
                    bounds = new Rectangle(0, 0, Width, height);
                else if (orientation == OrientationType.Bottom)
                    bounds = new Rectangle(0, Height - height, Width, height);
                else if (orientation == OrientationType.Left)
                    bounds = new Rectangle(0, 0, height, Height);
                else /*if (orientation == OrientationType.Right)*/
                    bounds = new Rectangle(Width - height, 0, height, Height);
                return bounds;
            }
        }

        private int TabRange
        {
            get { return (int)Math.Round(_inertialRange.Value); }
        }

        private IEnumerable<Renderer> GetTabRenderers()
        {
            // Display the selected item on top.
            _depItems.OnGet();
            _dynSelectedItem.OnGet();
            _dynOwnsImages.OnGet();
            OrientationType orientation = Orientation;
            foreach (object item in _items)
            {
                if (item != _selectedItem)
                    yield return new TabRenderer(_tabRendererContext, item, orientation, false, _ownsImages);
            }
            if (_selectedItem != null)
                yield return new TabRenderer(_tabRendererContext, _selectedItem, orientation, true, _ownsImages);
        }

        /// <summary>Not for external use</summary>
        /// <returns>Not for external use</returns>
        protected override IEnumerable<Renderer> GetRenderers()
        {
            yield return _glideScrollRenderer;
        }

        #endregion

        #region TabRenderer Context

        private class TabRendererContext : TabRenderer.Context
        {
            private ThemedTabDeck _owner;

            public TabRendererContext(ThemedTabDeck owner)
            {
                _owner = owner;
            }

            public string GetText(object item)
            {
                return _owner.GetText(item);
            }

            public bool HasImages
            {
                get { return _owner.HasImages; }
            }

            public Image GetImage(object item)
            {
                return _owner.GetImage(item);
            }

            public Theme Theme
            {
                get { return _owner.Theme; }
            }

            public Theme CloseTheme
            {
                get { return _owner.CloseTheme; }
            }

            public Size GetSize(object item)
            {
                return _owner.GetSize(item);
            }

            public Rectangle GetBounds(object item)
            {
                return _owner.GetBounds(item);
            }

            public void SelectItem(object item)
            {
                _owner.ChangeSelectedItem(item);
            }

            public void DragItem(object item, Point start, Point position)
            {
                _owner.DragItem(item, start, position);
            }

            public void EndDrag()
            {
                _owner.EndDrag();
            }

            public bool CanClose
            {
                get { return _owner.CanClose; }
            }

            public void OnClickClose(object item)
            {
                _owner.OnClickClose(item);
            }

            public void InvalidateRectangle(Rectangle rectangle)
            {
                _owner.Invalidate(_owner._glideScrollRenderer.TranslateRectangle(rectangle));
            }
        }

        private TabRendererContext _tabRendererContext;

        private string GetText(object item)
        {
            if (GetItemText != null)
                return GetItemText(item);
            else if (item == null)
                return string.Empty;
            else
                return item.ToString();
        }

        private Image GetImage(object item)
        {
            if (HasImages && GetItemImage != null)
                return GetItemImage(item);
            else
                return null;
        }

        private Rectangle GetBounds(object item)
        {
            _depInertialPosition.OnGet();
            InertialRectangle inertialRectangle;
            if (_inertialPosition.TryGetValue(item, out inertialRectangle))
                return inertialRectangle.Position;
            else
                return Rectangle.Empty;
        }

        private Size GetSize(object item)
        {
            OrientationType orientation = Orientation;
            int height = MeasureItemHeight();
            int size = MeasureItemWidth(item);
            if (CanClose)
                size += height;
            if (HasImages)
                size += height;
            int spacing = height / 4;

            if (orientation == OrientationType.Top || orientation == OrientationType.Bottom)
                return new Size(size + 2 * spacing, height);
            else /*if (orientation == OrientationType.Left || orientation == OrientationType.Right)*/
                return new Size(height, size + 2 * spacing);
        }

        private void OnClickClose(object item)
        {
            if (CloseTab != null)
            {
                if (Object.Equals(item, _selectedItem))
                {
                    // Select the next available tab.
                    if (_items.Count > 1)
                    {
                        int index = _items.IndexOf(item);
                        if (index >= 0)
                        {
                            if (index < _items.Count - 1)
                                SelectItem(_items[index + 1]);
                            else
                                SelectItem(_items[index - 1]);
                        }
                    }
                }

                // Close the tab.
                CloseTab(item);
            }
        }

        #endregion
    }
}
