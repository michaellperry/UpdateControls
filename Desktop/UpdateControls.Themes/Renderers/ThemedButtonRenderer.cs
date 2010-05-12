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
    public class ThemedButtonRenderer : UpdateControls.Themes.Renderers.Renderer
    {
        public abstract class Context
        {
            public abstract Theme Theme { get; }
            public virtual DefaultTheme.Style Style { get { return DefaultTheme.Style.Normal; } }
            public abstract Rectangle Rectangle { get; }
            public virtual Size Size { get { return Rectangle.Size; } }
            public virtual Rectangle HitRectangle { get { return Rectangle; } }
            public abstract bool Enabled { get; }
            public abstract bool Focused { get; }
            public abstract string Text { get; }
            public virtual void PaintButton(Graphics graphics, Rectangle bounds) { }
            public abstract void InvalidateRectangle(Rectangle bounds);
            public abstract void Click();
        };

        private Context _context;

        private SolidCache _disabledSolid;
        private SolidCache _normalSolid;
        private SolidCache _hoverSolid;
        private SolidCache _focusedSolid;
        private SolidCache _pressedSolid;

        private bool _inside = false;
        private bool _down = false;
        private bool _keyDown = false;
        private Independent _dynState = new Independent();

        private Image _image;
        private Rectangle _rectangle;
        private Rectangle _hitRectangle;
        private Dependent _depImage;
        private Dependent _depRectangle;
        private Dependent _depHitRectangle;

        public ThemedButtonRenderer(Context context, SolidShape shape)
        {
            _context = context;

            _disabledSolid = new SolidCache(shape, GetSize, GetDisabledDescriptor);
            _normalSolid = new SolidCache(shape, GetSize, GetNormalDescriptor);
            _hoverSolid = new SolidCache(shape, GetSize, GetHoverDescriptor);
            _focusedSolid = new SolidCache(shape, GetSize, GetFocusedDescriptor);
            _pressedSolid = new SolidCache(shape, GetSize, GetPressedDescriptor);

            // Create all dependent sentries.
            _depImage = new Dependent(UpdateImage);
            _depRectangle = new Dependent(UpdateRectangle);
            _depHitRectangle = new Dependent(UpdateHitRectangle);
        }

        private Rectangle Rectangle
        {
            get { _depRectangle.OnGet(); return _rectangle; }
        }

        private Rectangle HitRectangle
        {
            get { _depHitRectangle.OnGet(); return _hitRectangle; }
        }

        private Size GetSize()
        {
            return _context.Size;
        }

        private SolidDescriptor GetDisabledDescriptor()
        {
            return DefaultTheme.SolidDisabled(_context.Theme, _context.Style);
        }

        private SolidDescriptor GetNormalDescriptor()
        {
            return DefaultTheme.SolidRegular(_context.Theme, _context.Style);
        }

        private SolidDescriptor GetHoverDescriptor()
        {
            return DefaultTheme.SolidHover(_context.Theme, _context.Style);
        }

        private SolidDescriptor GetFocusedDescriptor()
        {
            return DefaultTheme.SolidFocused(_context.Theme, _context.Style);
        }

        private SolidDescriptor GetPressedDescriptor()
        {
            return DefaultTheme.SolidPressed(_context.Theme, _context.Style);
        }

        private void UpdateImage()
        {
            _dynState.OnGet();

            if (!_context.Enabled)
                _image = _disabledSolid.Image;
            else if ((_down && _inside) || _keyDown)
                _image = _pressedSolid.Image;
            else if (_context.Focused)
                _image = _focusedSolid.Image;
            else if (_inside)
                _image = _hoverSolid.Image;
            else
                _image = _normalSolid.Image;

            Theme theme = _context.Theme;
            if (theme != null)
            {
                Font dummyFont = theme.FontBody;
                Color dummyColor = theme.ColorBody;
            }

            _context.InvalidateRectangle(_rectangle);
        }

        private void UpdateRectangle()
        {
            Rectangle oldRectangle = _rectangle;
            _rectangle = _context.Rectangle;
            if (!oldRectangle.Equals(_rectangle))
            {
                // Invalidate the old and new rectangle.
                _context.InvalidateRectangle(oldRectangle);
                _context.InvalidateRectangle(_rectangle);
            }
        }

        private void UpdateHitRectangle()
        {
            _hitRectangle = _context.HitRectangle;
        }

        public override void Dispose()
        {
            _depImage.Dispose();
            _depRectangle.Dispose();
            _depHitRectangle.Dispose();
            _disabledSolid.Dispose();
            _normalSolid.Dispose();
            _hoverSolid.Dispose();
            _focusedSolid.Dispose();
            _pressedSolid.Dispose();

            base.Dispose();
        }

        public void OnKeyDown()
        {
            if (!_keyDown)
            {
                _dynState.OnSet();
                _keyDown = true;
            }
        }

        public bool OnKeyUp()
        {
            if (_keyDown)
            {
                _dynState.OnSet();
                _keyDown = false;
                return true;
            }
            return false;
        }

        public override void OnIdle()
        {
            _depImage.OnGet();
            _depRectangle.OnGet();
            _depHitRectangle.OnGet();

            base.OnIdle();
        }

        public override bool HitTest(Point mouse)
        {
            return HitRectangle.Contains(mouse);
        }

        public override void OnMouseEnter()
        {
            if (!_inside)
            {
                _dynState.OnSet();
                _inside = true;
            }

            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            if (_inside)
            {
                _dynState.OnSet();
                _inside = false;
            }

            base.OnMouseLeave();
        }

        public override void OnMouseDown(Point mouse, MouseButtons button, int clicks, Keys modifierKeys)
        {
            if (!_down)
            {
                _dynState.OnSet();
                _down = true;
            }

            base.OnMouseDown(mouse, button, clicks, modifierKeys);
        }

        public override void OnMouseDrag(Point start, Point mouse)
        {
            bool inside = HitTest(mouse);
            if ((inside && !_inside) || (!inside && _inside))
            {
                _dynState.OnSet();
                _inside = inside;
            }

            base.OnMouseDrag(start, mouse);
        }

        public override void OnMouseUp(Point start, Point mouse)
        {
            if (_down)
            {
                if (_inside)
                    _context.Click();
                _dynState.OnSet();
                _down = false;
            }

            base.OnMouseUp(start, mouse);
        }

        public override void PaintBackground(System.Drawing.Graphics graphics)
        {
            _depImage.OnGet();
            if (_image != null)
                graphics.DrawImage(_image, Rectangle.Location);

            // Paint the text.
            Theme theme = _context.Theme;
            Font font = DefaultTheme.FontBody(theme);
            Color color = DefaultTheme.ColorBody(theme);
            TextRenderer.DrawText(
                graphics,
                _context.Text,
                font,
                Rectangle,
                color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

            // Give the context a chance to paint.
            _context.PaintButton(graphics, Rectangle);

            base.PaintBackground(graphics);
        }
    }
}
