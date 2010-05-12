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
using System.Drawing;
using System.Windows.Forms;
using UpdateControls.Themes.Forms;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Renderers
{
    public class HeaderRenderer : UpdateControls.Themes.Renderers.Renderer
    {
        public interface Context
        {
            Theme Theme { get; }
            Rectangle Rectangle { get; }
            bool Enabled { get; }
            string Text { get; }
            bool Focus { get; }
            ContentAlignment TextAlign { get; }
            void InvalidateAll();
        };

        private Context _context;

        private SolidCache _disabledSolid;
        private SolidCache _normalSolid;
        private SolidCache _focusedSolid;

        private Image _image;
        private Rectangle _rectangle;
        private Dependent _depImage;
        private Dependent _depRectangle;

        public HeaderRenderer(Context context)
        {
            _context = context;

            _disabledSolid = new SolidCache(SolidShapeTile.Instance, GetSize, GetDisabledDescriptor);
            _normalSolid = new SolidCache(SolidShapeTile.Instance, GetSize, GetNormalDescriptor);
            _focusedSolid = new SolidCache(SolidShapeTile.Instance, GetSize, GetFocusedDescriptor);

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

        private SolidDescriptor GetDisabledDescriptor()
        {
            return DefaultTheme.SolidDisabled(_context.Theme);
        }

        private SolidDescriptor GetNormalDescriptor()
        {
            return DefaultTheme.SolidRegular(_context.Theme);
        }

        private SolidDescriptor GetFocusedDescriptor()
        {
            return DefaultTheme.SolidFocused(_context.Theme);
        }

        private void UpdateImage()
        {
            if (!_context.Enabled)
                _image = _disabledSolid.Image;
            else if (_context.Focus)
                _image = _focusedSolid.Image;
            else
                _image = _normalSolid.Image;

            Theme theme = _context.Theme;
            if (theme != null)
            {
                Font dummyFont = theme.FontHeader;
                Color dummyColor = theme.ColorHeader;
            }

            string dummyText = _context.Text;
            ContentAlignment dummyAlignment = _context.TextAlign;
            _context.InvalidateAll();
        }

        private void UpdateRectangle()
        {
            _rectangle = _context.Rectangle;
        }

        public override void Dispose()
        {
            _depImage.Dispose();
            _depRectangle.Dispose();
            _disabledSolid.Dispose();
            _normalSolid.Dispose();

            base.Dispose();
        }

        public override void OnIdle()
        {
            _depImage.OnGet();
            _depRectangle.OnGet();

            base.OnIdle();
        }

        public override void PaintBackground(System.Drawing.Graphics graphics)
        {
            _depImage.OnGet();
            if (_image != null)
                graphics.DrawImage(_image, Rectangle.Location);

            base.PaintBackground(graphics);
        }

        public override void PaintForeground(System.Drawing.Graphics graphics)
        {
            Theme theme = _context.Theme;
            Font font = DefaultTheme.FontHeader(theme);
            Color color = DefaultTheme.ColorHeader(theme);

            TextFormatFlags format = TextFormatFlags.WordBreak | new AlignmentFlags(_context.TextAlign).TextFormatFlags;
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
