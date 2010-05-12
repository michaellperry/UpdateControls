/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UpdateControls.Themes.Renderers;
using UpdateControls.Forms;

namespace UpdateControls.Themes.Forms
{
    [Description("A static text area whose appearance is controlled by a theme."),
    ToolboxBitmap(typeof(Header), "ToolboxImages.Header.png"),
    DefaultProperty("Theme"),
    DefaultEvent("GetText")]
    public partial class Header : RendererControl, IEnabledControl
    {
        /// <summary>
        /// Event fired to calculate the text to display.
        /// </summary>
        [Description("Event fired to calculate the text to display."), Category("Update")]
        public event GetStringDelegate GetText;
        /// <summary>
        /// Event fired to determine whether the control displays focus.
        /// </summary>
        [Description("Event fired to determine whether the control displays focus."), Category("Update")]
        public event GetBoolDelegate GetFocus;
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;

        private Dependent _depText;
        private Dependent _depEnabled;
        private Dependent _depDisplayFocus;

        private Theme _theme = null;
        private string _text = string.Empty;
        private bool _displayFocus = false;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private Independent _dynTheme = new Independent();
        private Independent _dynSize = new Independent();
        private Independent _dynText = new Independent();
        private Independent _dynTextAlign = new Independent();

        private class RendererContext : HeaderRenderer.Context
        {
            private Header _owner;

            public RendererContext(Header owner)
            {
                _owner = owner;
            }

            public Theme Theme
            {
                get { _owner._dynTheme.OnGet(); return _owner._theme; }
            }

            public Rectangle Rectangle
            {
                get { _owner._dynSize.OnGet(); return _owner.ClientRectangle; }
            }

            public bool Enabled
            {
                get { _owner._depEnabled.OnGet(); return _owner.Enabled; }
            }

            public ContentAlignment TextAlign
            {
                get { return _owner.TextAlign; }
            }

            public void InvalidateAll()
            {
                _owner.Invalidate();
            }

            public string Text
            {
                get { return _owner.Text; }
            }

            public bool Focus
            {
                get { return _owner.DisplayFocus; }
            }
        };

        private HeaderRenderer _renderer;

        public Header()
        {
            _depText = new Dependent(UpdateText);
            _depEnabled = new Dependent(UpdateEnabled);
            _depDisplayFocus = new Dependent(UpdateDisplayFocus);
            _renderer = new HeaderRenderer(new RendererContext(this));
            InitializeComponent();
        }

        protected override IEnumerable<Renderer> GetRenderers()
        {
            yield return _renderer;
        }

        [Category("Appearance"),
        Description("The set of properties that control the button's appearance.")]
        public Theme Theme
        {
            get { _dynTheme.OnGet(); return _theme; }
            set { _dynTheme.OnSet(); _theme = value; }
        }

        [Category("Appearance"),
        Description("The alignment of text within the header control."),
        DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get { _dynTextAlign.OnGet(); return _textAlign; }
            set { _dynTextAlign.OnSet(); _textAlign = value; }
        }

        private void UpdateText()
        {
            // Get the text from the event.
            if (GetText != null)
                base.Text = GetText();
            else // Or the property if there is no event.
            {
                _dynText.OnGet();
                base.Text = _text;
            }
        }

        private void UpdateEnabled()
        {
            // Get the property from the event.
            if (GetEnabled != null)
                base.Enabled = GetEnabled();
        }

        private void UpdateDisplayFocus()
        {
            // Get the property from the event.
            if (GetFocus != null)
                _displayFocus = GetFocus();
        }

        protected override void OnIdle()
        {
            _depText.OnGet();
            _depEnabled.OnGet();
            _depDisplayFocus.OnGet();
            base.OnIdle();
        }

        protected override void OnDestroy()
        {
            _depText.Dispose();
            _depEnabled.Dispose();
            _depDisplayFocus.Dispose();
            base.OnDestroy();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            _dynSize.OnSet();
            base.OnSizeChanged(e);
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

        public override string Text
        {
            get
            {
                if (_depText.IsNotUpdating)
                    _depText.OnGet();
                return base.Text;
            }
            set
            {
                _dynText.OnSet();
                _text = value;
            }
        }

        public bool DisplayFocus
        {
            get { _depDisplayFocus.OnGet(); return _displayFocus; }
        }
    }
}
