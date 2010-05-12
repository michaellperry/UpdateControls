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
using System.Drawing;
using UpdateControls.Themes.Solid;
using UpdateControls.Forms;

namespace UpdateControls.Themes.Forms
{
    [Description("A set of visual options shared by multiple controls."),
    ToolboxBitmap(typeof(Theme), "ToolboxImages.Theme.png"),
    DefaultProperty("SolidRegular")]
    public partial class Theme : Component
    {
        private SolidDescriptorEditable _solidDisabled;
        private SolidDescriptorEditable _solidRegular;
        private SolidDescriptorEditable _solidHover;
        private SolidDescriptorEditable _solidFocused;
        private SolidDescriptorEditable _solidPressed;
        private Font _fontHeader;
        private Font _fontBody;
        private Color _colorHeader = DefaultTheme.DefaultColorHeader;
        private Color _colorBody = DefaultTheme.DefaultColorBody;
        private Color _colorEven = DefaultTheme.DefaultColorEven;
        private Color _colorOdd = DefaultTheme.DefaultColorOdd;
        private Color _colorSelected = DefaultTheme.DefaultColorSelected;

        private Independent _dynSolidDisabled = new Independent();
        private Independent _dynSolidRegular = new Independent();
        private Independent _dynSolidHover = new Independent();
        private Independent _dynSolidFocused = new Independent();
        private Independent _dynSolidPressed = new Independent();
        private Independent _dynFontHeader = new Independent();
        private Independent _dynFontBody = new Independent();
        private Independent _dynColorHeader = new Independent();
        private Independent _dynColorBody = new Independent();
        private Independent _dynColorEven = new Independent();
        private Independent _dynColorOdd = new Independent();
        private Independent _dynColorSelected = new Independent();

        public Theme() : this(null)
        {
        }

        public Theme(IContainer container)
        {
            if (container != null)
                container.Add(this);

            _solidDisabled = new SolidDescriptorEditable(DefaultTheme.DefaultNormalDisabled);
            _solidRegular = new SolidDescriptorEditable(DefaultTheme.DefaultNormalRegular);
            _solidHover = new SolidDescriptorEditable(DefaultTheme.DefaultNormalHover);
            _solidFocused = new SolidDescriptorEditable(DefaultTheme.DefaultNormalFocused);
            _solidPressed = new SolidDescriptorEditable(DefaultTheme.DefaultNormalPressed);

            _fontHeader = DefaultTheme.DefaultFontHeader;
            _fontBody = DefaultTheme.DefaultFontBody;

            InitializeComponent();
        }

        [Description("The appearance of a disabled solid."), Category("Appearance")]
        [Browsable(true)]
        [EditorAttribute(typeof(SolidDescriptorEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public SolidDescriptorEditable SolidDisabled
        {
            get
            {
                _dynSolidDisabled.OnGet();
                return _solidDisabled;
            }
            set
            {
                _dynSolidDisabled.OnSet();
                _solidDisabled = value;
            }
        }

        [Description("The appearance of a regular solid."), Category("Appearance")]
        [Browsable(true)]
        [EditorAttribute(typeof(SolidDescriptorEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public SolidDescriptorEditable SolidRegular
        {
            get
            {
                _dynSolidRegular.OnGet();
                return _solidRegular;
            }
            set
            {
                _dynSolidRegular.OnSet();
                _solidRegular = value;
            }
        }

        [Description("The appearance of a solid under the mouse."), Category("Appearance")]
        [Browsable(true)]
        [EditorAttribute(typeof(SolidDescriptorEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public SolidDescriptorEditable SolidHover
        {
            get
            {
                _dynSolidHover.OnGet();
                return _solidHover;
            }
            set
            {
                _dynSolidHover.OnSet();
                _solidHover = value;
            }
        }

        [Description("The appearance of a solid that has keyboard focus or is otherwise selected."), Category("Appearance")]
        [Browsable(true)]
        [EditorAttribute(typeof(SolidDescriptorEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public SolidDescriptorEditable SolidFocused
        {
            get
            {
                _dynSolidFocused.OnGet();
                return _solidFocused;
            }
            set
            {
                _dynSolidFocused.OnSet();
                _solidFocused = value;
            }
        }

        [Description("The appearance of a solid when pressed."), Category("Appearance")]
        [Browsable(true)]
        [EditorAttribute(typeof(SolidDescriptorEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public SolidDescriptorEditable SolidPressed
        {
            get
            {
                _dynSolidPressed.OnGet();
                return _solidPressed;
            }
            set
            {
                _dynSolidPressed.OnSet();
                _solidPressed = value;
            }
        }

        [Description("The font used for header text"),
        Category("Appearance")]
        public Font FontHeader
        {
            get
            {
                _dynFontHeader.OnGet();
                return _fontHeader;
            }
            set
            {
                _dynFontHeader.OnSet();
                _fontHeader = value;
            }
        }

        [Description("The font used for body text"),
        Category("Appearance")]
        public Font FontBody
        {
            get
            {
                _dynFontBody.OnGet();
                return _fontBody;
            }
            set
            {
                _dynFontBody.OnSet();
                _fontBody = value;
            }
        }

        [Description("The color used for header text"),
        Category("Appearance")]
        public Color ColorHeader
        {
            get
            {
                _dynColorHeader.OnGet();
                return _colorHeader;
            }
            set
            {
                _dynColorHeader.OnSet();
                _colorHeader = value;
            }
        }

        [Description("The color used for body text"),
        Category("Appearance")]
        public Color ColorBody
        {
            get
            {
                _dynColorBody.OnGet();
                return _colorBody;
            }
            set
            {
                _dynColorBody.OnSet();
                _colorBody = value;
            }
        }

        [Description("The color used for even backgrounds"),
        Category("Appearance")]
        public Color ColorEven
        {
            get
            {
                _dynColorEven.OnGet();
                return _colorEven;
            }
            set
            {
                _dynColorEven.OnSet();
                _colorEven = value;
            }
        }

        [Description("The color used for odd backgrounds"),
        Category("Appearance")]
        public Color ColorOdd
        {
            get
            {
                _dynColorOdd.OnGet();
                return _colorOdd;
            }
            set
            {
                _dynColorOdd.OnSet();
                _colorOdd = value;
            }
        }

        [Description("The color used for selected backgrounds"),
        Category("Appearance")]
        public Color ColorSelected
        {
            get
            {
                _dynColorSelected.OnGet();
                return _colorSelected;
            }
            set
            {
                _dynColorSelected.OnSet();
                _colorSelected = value;
            }
        }
    }
}
