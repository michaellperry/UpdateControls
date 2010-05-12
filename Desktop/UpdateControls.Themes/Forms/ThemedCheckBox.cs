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
using System.Drawing.Drawing2D;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Forms
{
    [Description("A checkbox whose appearance is controlled by a theme."),
    ToolboxBitmap(typeof(ThemedCheckBox), "ToolboxImages.ThemedCheckBox.png"),
    DefaultProperty("Theme"),
	DefaultEvent("GetChecked")]
    public partial class ThemedCheckBox : RendererControl, IEnabledControl
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
        [Description("Event fired to calculate whether the box is checked."), Category("Update")]
        public event GetBoolDelegate GetChecked;
        /// <summary>
        /// Event fired when the box is checked or unchecked.
        /// </summary>
        [Description("Event fired when the box is checked or unchecked."), Category("Update")]
        public event SetBoolDelegate SetChecked;
        /// <summary>
        /// Event fired to calculate the check state of the three-state box.
        /// </summary>
        [Description("Event fired to calculate the check state of the three-state box."), Category("Update")]
        public event GetCheckStateDelegate GetCheckState;
        /// <summary>
        /// Event fired when the state of the three-state box is changed.
        /// </summary>
        [Description("Event fired when the state of the three-state box is changed."), Category("Update")]
        public event SetCheckStateDelegate SetCheckState;

        private int _textHeight;
        private CheckState _checkState = CheckState.Unchecked;
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
        private Independent _dynTextAlign = new Independent();
        private Independent _dynCheckAlign = new Independent();
        private Independent _dynSize = new Independent();
        private Independent _dynFocused = new Independent();
        private Independent _dynAccept = new Independent();

        private class ButtonRendererContext : ThemedButtonRenderer.Context
        {
            private ThemedCheckBox _themedCheckBox;

            public ButtonRendererContext(ThemedCheckBox themedButton)
            {
                _themedCheckBox = themedButton;
            }

            public override Theme Theme
            {
                get { return _themedCheckBox.Theme; }
            }

            public override DefaultTheme.Style Style
            {
                get { return _themedCheckBox.ThemeStyle; }
            }

            public override Rectangle Rectangle
            {
                get
                {
                    Rectangle buttonRectangle = _themedCheckBox.ButtonRectangle;
                    buttonRectangle.Inflate(-buttonRectangle.Width / 5, -buttonRectangle.Height / 5);
                    return buttonRectangle;
                }
            }

            public override Rectangle HitRectangle
            {
                get { _themedCheckBox._dynSize.OnGet(); return _themedCheckBox.ClientRectangle; }
            }

            public override bool Enabled
            {
                get { _themedCheckBox._depEnabled.OnGet(); return _themedCheckBox.Enabled; }
            }

            public override bool Focused
            {
                get { _themedCheckBox._dynFocused.OnGet(); _themedCheckBox._dynAccept.OnGet(); return _themedCheckBox.Focused || _themedCheckBox._accept; }
            }

            public override void InvalidateRectangle(Rectangle rect)
            {
                _themedCheckBox.Invalidate(rect);
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
            private ThemedCheckBox _themedCheckBox;

            public TextRendererContext(ThemedCheckBox themedButton)
            {
                _themedCheckBox = themedButton;
            }

            public Rectangle Rectangle
            {
                get { return _themedCheckBox.TextRectangle; }
            }

            public void InvalidateRectangle(Rectangle rectangle)
            {
                _themedCheckBox.Invalidate(rectangle);
            }

            public Theme Theme
            {
                get { return _themedCheckBox.UncheckedTheme; }
            }

            public bool Enabled
            {
                get { return _themedCheckBox.Enabled; }
            }

            public string Text
            {
                get { return _themedCheckBox.Text; }
            }

            public AlignmentFlags Alignment
            {
                get { return new AlignmentFlags(_themedCheckBox.TextAlign); }
            }
        }

        private StaticTextRenderer _textRenderer;

        public ThemedCheckBox()
        {
            _depEnabled = new Dependent(UpdateEnabled);
            _depTextHeight = new Dependent(UpdateTextHeight);
            _depChecked = new Dependent(UpdateChecked);
            _buttonRenderer = new ThemedButtonRenderer(new ButtonRendererContext(this), SolidShapeTile.Instance);
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
            // Call the most discriminating getter available.
            if (GetCheckState != null)
                _checkState = GetCheckState();
            else if (GetChecked != null)
                _checkState = GetChecked() ? CheckState.Checked : CheckState.Unchecked;
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
                if (_checkState == CheckState.Unchecked)
                    return UncheckedTheme;
                else
                    return CheckedTheme;
            }
        }

        private DefaultTheme.Style ThemeStyle
        {
            get
            {
                _depChecked.OnGet();
                if (_checkState == CheckState.Unchecked)
                    return DefaultTheme.Style.Unchecked;
                else
                    return DefaultTheme.Style.Normal;
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
            if (Enabled)
            {
                // Call the most discriminating setter available.
                if (SetCheckState != null)
                    SetCheckState(
                        (_checkState == CheckState.Checked) ? CheckState.Indeterminate :
                        (_checkState == CheckState.Indeterminate) ? CheckState.Unchecked :
                        /*(_checkState == CheckState.Unchecked) ?*/ CheckState.Checked);
                else if (SetChecked != null)
                    SetChecked(_checkState == CheckState.Unchecked);
            }
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Paint the text and button first.
            base.OnPaint(pe);

            // If checked, add a check mark.
            if (_checkState == CheckState.Checked)
            {
                using (Pen pen = new Pen(DefaultTheme.ColorBody(Theme), 0.1f * ButtonRectangle.Width))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        RectangleF buttonRectangle = ButtonRectangle;
                        path.AddLines(new PointF[]
                        {
                            new PointF(
                                0.75f * buttonRectangle.Left + 0.25f * buttonRectangle.Right,
                                0.50f * buttonRectangle.Top + 0.50f * buttonRectangle.Bottom),
                            new PointF(
                                0.67f * buttonRectangle.Left + 0.33f * buttonRectangle.Right,
                                0.30f * buttonRectangle.Top + 0.70f * buttonRectangle.Bottom),
                            new PointF(
                                0.15f * buttonRectangle.Left + 0.85f * buttonRectangle.Right,
                                0.85f * buttonRectangle.Top + 0.15f * buttonRectangle.Bottom),
                        });
                        pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        pe.Graphics.DrawPath(pen, path);
                    }
                }
            }
            else if (_checkState == CheckState.Indeterminate)
            {
                using (Brush brush = new SolidBrush(DefaultTheme.ColorBody(Theme)))
                {
                    RectangleF fill = ButtonRectangle;
                    fill.Inflate(-0.25f * fill.Width, -0.25f * fill.Height);
                    pe.Graphics.FillRectangle(brush, fill);
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
