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
using System.Text;
using UpdateControls.Themes.Renderers;
using System.ComponentModel;
using System.Drawing;
using UpdateControls.Forms;

namespace UpdateControls.Themes.Forms
{
    [Description("A logo whose appearance is controlled by a theme."),
    ToolboxBitmap(typeof(Logo), "ToolboxImages.Logo.png"),
    DefaultProperty("Theme")]
    public class Logo : RendererControl
    {
        public static readonly Color DefaultHelixColor = Color.FromArgb(255, 206, 0);

        private Theme _theme = null;
        private Color _helixColor = DefaultHelixColor;
        private Independent _dynTheme = new Independent();
        private Independent _dynHelixColor = new Independent();
        private Independent _dynSize = new Independent();

        private class LogoRendererContext : LogoRenderer.Context
        {
            private Logo _owner;

            public LogoRendererContext(Logo owner)
            {
                _owner = owner;
            }

            #region Context Members

            public Theme Theme
            {
                get { return _owner.Theme; }
            }

            public Color HelixColor
            {
                get { return _owner.HelixColor; }
            }

            public Rectangle Rectangle
            {
                get { _owner._dynSize.OnGet(); return _owner.ClientRectangle; }
            }

            public void Invalidate(Rectangle rectangle)
            {
                _owner.Invalidate(rectangle);
            }

            #endregion
        }

        public Logo()
        {
        }

        [Category("Appearance"),
        Description("The set of properties that control the button's appearance.")]
        public Theme Theme
        {
            get { _dynTheme.OnGet(); return _theme; }
            set { _dynTheme.OnSet(); _theme = value; }
        }

        [Category("Appearance"),
        Description("The color of the helix.")]
        public Color HelixColor
        {
            get { _dynHelixColor.OnGet(); return _helixColor; }
            set { _dynHelixColor.OnSet(); _helixColor = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            _dynSize.OnSet();
            base.OnSizeChanged(e);
        }

        protected override IEnumerable<Renderer> GetRenderers()
        {
            yield return new LogoRenderer(new LogoRendererContext(this));
        }
    }
}
