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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using UpdateControls.Themes.Renderers;
using UpdateControls.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Forms
{
    [Description("A radio button whose appearance is controlled by a theme."),
    ToolboxBitmap(typeof(ThemedRadioButton), "ToolboxImages.ThemedRadioButton.png"),
    DefaultProperty("Theme"),
    DefaultEvent("GetChecked")]
    public partial class ThemedRadioButton : RendererControl, IEnabledControl
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
        /// <summary>
        /// Event fired to calculate whether the box is checked.
        /// </summary>
        [Description("Event fired to calculate whether the radio button is checked."), Category("Update")]
        public event GetBoolDelegate GetChecked;
        /// <summary>
        /// Event fired when the box is checked or unchecked.
        /// </summary>
        [Description("Event fired when the radio button is checked."), Category("Update")]
        public event ActionDelegate SetChecked;

        private int _textHeight;
        private bool _checked = false;
        private Dependent _depEnabled;
        private Dependent _depTextHeight;
        private Dependent _depChecked;

        private Theme _uncheckedTheme = null;
        private Theme _checkedTheme = null;
        private ContentAlignment _checkAlign = ContentAlignment.MiddleLeft;
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
        private bool _accept = false;
        private Independent _dynUncheckedTheme = new Independent();
        private Independent _dynCheckedTheme = new Independent();
        private Independent _dynCheckAlign = new Independent();
        private Independent _dynTextAlign = new Independent();
        private Independent _dynSize = new Independent();
        private Independent _dynFocused = new Independent();
        private Independent _dynAccept = new Independent();

        private class ButtonRendererContext : ThemedButtonRenderer.Context
        {
            private ThemedRadioButton _themedRadioButton;

            public ButtonRendererContext(ThemedRadioButton themedRadioButton)
            {
                _themedRadioButton = themedRadioButton;
            }

            public override Theme Theme
            {
                get { return _themedRadioButton.Theme; }
            }

            public override DefaultTheme.Style Style
            {
                get { return _themedRadioButton.ThemeStyle; }
            }

            public override Rectangle Rectangle
            {
                get
                {
                    Rectangle buttonRectangle = _themedRadioButton.ButtonRectangle;
                    buttonRectangle.Inflate(-buttonRectangle.Width / 5, -buttonRectangle.Height / 5);
                    return buttonRectangle;
                }
            }

            public override Rectangle HitRectangle
            {
                get { _themedRadioButton._dynSize.OnGet(); return _themedRadioButton.ClientRectangle; }
            }

            public override bool Enabled
            {
                get { _themedRadioButton._depEnabled.OnGet(); return _themedRadioButton.Enabled; }
            }

            public override bool Focused
            {
                get { _themedRadioButton._dynFocused.OnGet(); _themedRadioButton._dynAccept.OnGet(); return _themedRadioButton.Focused || _themedRadioButton._accept; }
            }

            public override void InvalidateRectangle(Rectangle rect)
            {
                _themedRadioButton.Invalidate(rect);
            }

            public override void Click()
            {
                // Control converts the mouse gestures into clicks. We don't have to.
            }

            public override string Text
            {
                get { return string.Empty; }
            }
        };

        private ThemedButtonRenderer _buttonRenderer;

        private class TextRendererContext : StaticTextRenderer.Context
        {
            private ThemedRadioButton _themedRadioButton;

            public TextRendererContext(ThemedRadioButton themedRadioButton)
            {
                _themedRadioButton = themedRadioButton;
            }

            public Rectangle Rectangle
            {
                get { return _themedRadioButton.TextRectangle; }
            }

            public void InvalidateRectangle(Rectangle rectangle)
            {
                _themedRadioButton.Invalidate(rectangle);
            }

            public Theme Theme
            {
                get { return _themedRadioButton.UncheckedTheme; }
            }

            public bool Enabled
            {
                get { return _themedRadioButton.Enabled; }
            }

            public string Text
            {
                get { return _themedRadioButton.Text; }
            }

            public AlignmentFlags Alignment
            {
                get { return new AlignmentFlags(_themedRadioButton.TextAlign); }
            }
        }

        private StaticTextRenderer _textRenderer;

        public ThemedRadioButton()
        {
            _depEnabled = new Dependent(UpdateEnabled);
            _depTextHeight = new Dependent(UpdateTextHeight);
            _depChecked = new Dependent(UpdateChecked);
            _buttonRenderer = new ThemedButtonRenderer(new ButtonRendererContext(this), SolidShapeEllipse.Instance);
            _textRenderer = new StaticTextRenderer(new TextRendererContext(this));
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
            InitializeComponent();
        }

        protected override IEnumerable<Renderer> GetRenderers()
        {
            yield return _buttonRenderer;
            yield return _textRenderer;
        }

        [Category("Appearance"),
        Description("The set of properties that control the button's appearance while not checked.")]
        public Theme UncheckedTheme
        {
            get { _dynUncheckedTheme.OnGet(); return _uncheckedTheme; }
            set { _dynUncheckedTheme.OnSet(); _uncheckedTheme = value; }
        }

        [Category("Appearance"),
        Description("The set of properties that control the button's appearance while checked.")]
        public Theme CheckedTheme
        {
            get { _dynCheckedTheme.OnGet(); return _checkedTheme; }
            set { _dynCheckedTheme.OnSet(); _checkedTheme = value; }
        }

        [Category("Appearance"),
        Description("Determines the location of the checkbox inside the control."),
        DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment CheckAlign
        {
            get { _dynCheckAlign.OnGet(); return _checkAlign; }
            set { _dynCheckAlign.OnSet(); _checkAlign = value; }
        }

        [Category("Appearance"),
        Description("Determines the alignment of text."),
        DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlign
        {
            get { _dynTextAlign.OnGet(); return _textAlign; }
            set { _dynTextAlign.OnSet(); _textAlign = value; }
        }

        private void UpdateEnabled()
        {
            // Get the property from the event.
            if (GetEnabled != null)
                base.Enabled = GetEnabled();
        }

        private void UpdateTextHeight()
        {
            // Measure the text.
            Font font = DefaultTheme.FontBody(UncheckedTheme);
            _textHeight = TextRenderer.MeasureText("0", font).Height;
        }

        private void UpdateChecked()
        {
            if (GetChecked != null)
            {
                _checked = GetChecked();
                base.TabStop = _checked;
            }
        }

        private Rectangle ButtonRectangle
        {
            get
            {
                _dynSize.OnGet();
                _dynCheckAlign.OnGet();
                _depTextHeight.OnGet();
                return new AlignmentFlags(_checkAlign).AlignRectangle(ClientRectangle, new Size(_textHeight, _textHeight));
            }
        }

        private Rectangle TextRectangle
        {
            get
            {
                _dynSize.OnGet();
                _dynCheckAlign.OnGet();
                _depTextHeight.OnGet();
                AlignmentFlags alignment = new AlignmentFlags(_checkAlign);
                Rectangle result = ClientRectangle;
                if (alignment.Horizontal == AlignmentFlags.HorizontalAlignment.Left)
                {
                    result.X += _textHeight;
                    result.Width -= _textHeight;
                }
                else if (alignment.Horizontal == AlignmentFlags.HorizontalAlignment.Right)
                {
                    result.Width -= _textHeight;
                }
                if (alignment.Vertical == AlignmentFlags.VerticalAlignment.Top)
                {
                    result.Y += _textHeight;
                    result.Height -= _textHeight;
                }
                else if (alignment.Vertical == AlignmentFlags.VerticalAlignment.Bottom)
                {
                    result.Height -= _textHeight;
                }
                return result;
            }
        }

        private Theme Theme
        {
            get
            {
                _depChecked.OnGet();
                if (_checked)
                    return CheckedTheme;
                else
                    return UncheckedTheme;
            }
        }

        private DefaultTheme.Style ThemeStyle
        {
            get
            {
                _depChecked.OnGet();
                if (_checked)
                    return DefaultTheme.Style.Normal;
                else
                    return DefaultTheme.Style.Unchecked;
            }
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
            if (!_checked && Enabled && SetChecked != null)
                SetChecked();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _dynFocused.OnSet();
            _buttonRenderer.OnKeyUp();
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                _buttonRenderer.OnKeyDown();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                if (_buttonRenderer.OnKeyUp())
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

        protected override void OnClick(EventArgs e)
        {
            if (Enabled && SetChecked != null)
                SetChecked();
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Paint the text and button first.
            base.OnPaint(pe);

            // If checked, add a circle.
            if (_checked)
            {
                using (Brush brush = new SolidBrush(DefaultTheme.ColorBody(Theme)))
                {
                    RectangleF fill = ButtonRectangle;
                    fill.Inflate(-0.35f * fill.Width, -0.35f * fill.Height);
                    pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    pe.Graphics.FillEllipse(brush, fill);
                }
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
    }
}
