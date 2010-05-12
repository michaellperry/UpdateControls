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
using UpdateControls.Themes.Forms;
using UpdateControls.Themes.Renderers;
using UpdateControls.Forms;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Forms
{
    [Description("A button whose appearance is controlled by a theme."),
    ToolboxBitmap(typeof(ThemedButton), "ToolboxImages.ThemedButton.png"),
    DefaultProperty("Theme"),
	DefaultEvent("Click")]
    public partial class ThemedButton : RendererControl, IButtonControl, IEnabledControl
    {
        /// <summary>Event fired to determine whether the control is enabled.</summary>
        /// <remarks>
        /// Return true when the control should be enabled, and false when it should not be. If
        /// this event is not handled, the control is always enabled.
        /// </remarks>
        [Description("Event fired to determine whether the control is enabled."), Category("Update")]
		public event GetBoolDelegate GetEnabled;
        [Browsable(false)]
        public new event EventHandler DoubleClick;

        private Dependent _depEnabled;

        private DialogResult _dialogResult = DialogResult.None;

        private Theme _theme = null;
        private bool _accept = false;
        private Independent _dynTheme = new Independent();
        private Independent _dynSize = new Independent();
        private Independent _dynFocused = new Independent();
        private Independent _dynAccept = new Independent();

        private class RendererContext : ThemedButtonRenderer.Context
        {
            private ThemedButton _themedButton;

            public RendererContext(ThemedButton themedButton)
            {
                _themedButton = themedButton;
            }

            public override Theme Theme
            {
                get { _themedButton._dynTheme.OnGet(); return _themedButton._theme; }
            }

            public override Rectangle Rectangle
            {
                get { _themedButton._dynSize.OnGet(); return _themedButton.ClientRectangle; }
            }

            public override bool Enabled
            {
                get { _themedButton._depEnabled.OnGet(); return _themedButton.Enabled; }
            }

            public override bool Focused
            {
                get { _themedButton._dynFocused.OnGet(); _themedButton._dynAccept.OnGet(); return _themedButton.Focused || _themedButton._accept; }
            }

            public override void InvalidateRectangle(Rectangle rect)
            {
                _themedButton.Invalidate(rect);
            }

            public override void Click()
            {
                // Control converts the mouse gestures into clicks. We don't have to.
            }

            public override string Text
            {
                get { return _themedButton.Text; }
            }
        };

        private ThemedButtonRenderer _renderer;

        public ThemedButton()
        {
            _depEnabled = new Dependent(UpdateEnabled);
            _renderer = new ThemedButtonRenderer(new RendererContext(this), SolidShapeLozenge.Instance);
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
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

        private void UpdateEnabled()
        {
            // Get the property from the event.
            if (GetEnabled != null)
                base.Enabled = GetEnabled();
        }

        protected override void OnIdle()
        {
            _depEnabled.OnGet();
            base.OnIdle();
        }

        protected override void OnDestroy()
        {
            _depEnabled.Dispose();
            base.OnDestroy();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _dynFocused.OnSet();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _renderer.OnKeyUp();
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                _renderer.OnKeyDown();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                if (_renderer.OnKeyUp())
                    OnClick(new EventArgs());
            }
            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            _dynSize.OnSet();
            base.OnSizeChanged(e);
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            if (base.CanSelect && Control.IsMnemonic(charCode, base.Text))
            {
                OnClick(new EventArgs());
                return true;
            }
            else
                return false;
        }

        #region IButtonControl Members

        [DefaultValue(DialogResult.None),
        Category("Behavior"),
        Description("The dialog-box result produced in a modal form by clicking the button.")]
        public DialogResult DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
            }
        }

        public void NotifyDefault(bool value)
        {
            if ((_accept && !value) || (!_accept && value))
            {
                _dynAccept.OnSet();
                _accept = value;
            }
        }

        public void PerformClick()
        {
            OnClick(new EventArgs());
        }

        #endregion

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
                return base.Text;
            }
            set
            {
                Invalidate();
                base.Text = value;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (base.Enabled)
            {
                if (_dialogResult != DialogResult.None)
                {
                    Form form = FindForm();
                    if (form != null)
                        form.DialogResult = _dialogResult;
                }
                base.OnClick(e);
            }
        }
    }
}
