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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using UpdateControls.Themes.Forms;

namespace UpdateControls.Themes.Renderers
{
    public class StaticTextRenderer : UpdateControls.Themes.Renderers.Renderer
    {
        public interface Context
        {
            Theme Theme { get; }
            Rectangle Rectangle { get; }
            bool Enabled { get; }
            string Text { get; }
            AlignmentFlags Alignment { get; }
            void InvalidateRectangle(Rectangle rectangle);
        };

        private Context _context;

        private Rectangle _rectangle;
        private Dependent _depImage;
        private Dependent _depRectangle;

        public StaticTextRenderer(Context context)
        {
            _context = context;

            // Create all dependent sentries.
            _depImage = new Dependent(UpdateImage);
            _depRectangle = new Dependent(UpdateRectangle);
        }

        private Rectangle Rectangle
        {
            get { _depRectangle.OnGet(); return _rectangle; }
        }

        private Size GetSize()
        {
            return Rectangle.Size;
        }

        private void UpdateImage()
        {
            Theme theme = _context.Theme;
            Font dummyFont = DefaultTheme.FontBody(theme);
            Color dummyColor = DefaultTheme.ColorBody(theme);
            string dummyText = _context.Text;

            _context.InvalidateRectangle(_rectangle);
        }

        private void UpdateRectangle()
        {
            Rectangle previous = _rectangle;
            _rectangle = _context.Rectangle;

            _context.InvalidateRectangle(previous);
            if (_rectangle != previous)
                _context.InvalidateRectangle(_rectangle);
        }

        public override void Dispose()
        {
            _depImage.Dispose();
            _depRectangle.Dispose();

            base.Dispose();
        }

        public override void OnIdle()
        {
            _depImage.OnGet();
            _depRectangle.OnGet();

            base.OnIdle();
        }

        public override void PaintForeground(System.Drawing.Graphics graphics)
        {
            Theme theme = _context.Theme;
            Font font = DefaultTheme.FontBody(theme);
            Color color = DefaultTheme.ColorBody(theme);

            TextFormatFlags format = TextFormatFlags.WordBreak | _context.Alignment.TextFormatFlags;
            TextRenderer.DrawText(
                graphics,
                _context.Text,
                font,
                Rectangle,
                color,
                format);

            base.PaintForeground(graphics);
        }
    }
}
