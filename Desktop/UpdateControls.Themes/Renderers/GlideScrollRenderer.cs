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
using UpdateControls.Themes.Inertia;
using UpdateControls.Themes.GraphicsUtil;
using System.Runtime.InteropServices;

namespace UpdateControls.Themes.Renderers
{
    public class GlideScrollRenderer : Renderer
    {
        #region Context

        public interface Context
        {
            // Get the bounds in which the glide scrolling functions.
            Rectangle Bounds { get; }
            // Get the range of the glide scrolling.
            int Range { get; }
            // True if the glide scroller is horizontal. False if it is vertical.
            bool Horizontal { get; }
            // Produce the collection of renders to scroll.
            IEnumerable<Renderer> GetChildRenderers();
            // Invalidate a section of the control.
            void Invalidate(Rectangle bounds);

            // The window handle for fast scrolling.
            IntPtr Handle { get; }
        }
        private Context _context;

        #endregion

        #region GlideScroller

        private class GlideScrollerContext : GlideScroller.Context
        {
            private GlideScrollRenderer _owner;

            public GlideScrollerContext(GlideScrollRenderer owner)
            {
                _owner = owner;
            }

            public int Range
            {
                get { return _owner._context.Range; }
            }

            public int Width
            {
                get { return _owner._context.Horizontal ?
                    _owner._context.Bounds.Width :
                    _owner._context.Bounds.Height; }
            }
        }

        private GlideScroller _glideScroller;

        #endregion

        #region Construction and destruction

        private RendererController _rendererController;
        private Rectangle _bounds = Rectangle.Empty;
        private Dependent _depBounds;
        private Dependent _depPaint;

        public GlideScrollRenderer(Context context)
        {
            _context = context;
            _glideScroller = new GlideScroller(new GlideScrollerContext(this));
            _rendererController = new RendererController(GetChildRenderers);
            _depBounds = new Dependent(UpdateBounds);
            _depPaint = new Dependent(UpdatePaint);
        }

        public override void OnIdle()
        {
            _rendererController.OnIdle();
            _depBounds.OnGet();
            _depPaint.OnGet();
            base.OnIdle();
        }

        public override void Dispose()
        {
            _glideScroller.Dispose();
            _rendererController.Dispose();
            _depBounds.Dispose();
            _depPaint.Dispose();
            base.Dispose();
        }

        #endregion

        #region Hide child renderers

        // Hide all child renderers. We manage them ourselves.
        public override IEnumerable<Renderer> ChildRenderers
        {
            get { yield break; }
        }

        public override IEnumerable<Renderer> TopDown
        {
            get { yield break; }
        }

        public override IEnumerable<Renderer> BottomUp
        {
            get { yield break; }
        }

        #endregion

        #region Mouse messages

        public override bool HitTest(Point mouse)
        {
            _depBounds.OnGet();
            return _bounds.Contains(mouse);
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            _glideScroller.Exit();
            _rendererController.OnMouseLeave();
            base.OnMouseLeave();
        }

        public override void OnMouseMove(Point mouse)
        {
            _glideScroller.Enter(GetCoordinate(mouse));
            _rendererController.OnMouseMove(TranslateMouse(mouse));
            base.OnMouseMove(mouse);
        }

        public override void OnMouseDown(Point mouse, System.Windows.Forms.MouseButtons button, int clicks, System.Windows.Forms.Keys modifierKeys)
        {
            _rendererController.OnMouseDown(TranslateMouse(mouse), button, clicks, modifierKeys);
            base.OnMouseDown(mouse, button, clicks, modifierKeys);
        }

        public override void OnMouseUp(Point start, Point mouse)
        {
            _rendererController.OnMouseUp(TranslateMouse(mouse));
            base.OnMouseUp(start, mouse);
        }

        public override void OnMouseDrag(Point start, Point mouse)
        {
            _glideScroller.Enter(GetCoordinate(mouse));
            _rendererController.OnMouseMove(TranslateMouse(mouse));
            base.OnMouseDrag(start, mouse);
        }

        public override void OnMouseDragTimer()
        {
            _rendererController.OnTimer();
            base.OnMouseDragTimer();
        }

        #endregion

        #region Painting

        private int _position = -1;

        public override void PaintBackground(Graphics graphics)
        {
            using (new GraphicsStateSaver(graphics))
            {
                graphics.SetClip(_bounds);
                TranslateGraphics(graphics);
                _rendererController.PaintBackground(graphics);
            }
            base.PaintBackground(graphics);
        }

        public override void PaintForeground(Graphics graphics)
        {
            using (new GraphicsStateSaver(graphics))
            {
                graphics.SetClip(_bounds);
                TranslateGraphics(graphics);
                _rendererController.PaintForeground(graphics);
            }
            base.PaintForeground(graphics);
        }

        private void UpdateBounds()
        {
            Rectangle oldBounds = _bounds;
            _bounds = _context.Bounds;
            if (!oldBounds.Equals(_bounds))
            {
                // Invalidate the old and new bounds.
                _context.Invalidate(oldBounds);
                _context.Invalidate(_bounds);
            }
        }

        [DllImport("user32")]
        public static extern int ScrollDC(int hdc, int dx, int dy, ref Rectangle lprcScroll, ref Rectangle lprcClip, int hrgnUpdate, ref Rectangle lprcUpdate);
        [DllImport("user32")]
        public static extern int GetDC(int hwnd);
        [DllImport("user32")]
        public static extern int UpdateWindow(int hwnd);
        private void UpdatePaint()
        {
            // React to changes in scroll position.
            int oldPosition = _position;
            _position = _glideScroller.Position;
            if (oldPosition >= 0 && _position != oldPosition)
            {
                // Quickly scroll the image.
                int hwnd = (int)_context.Handle;
                int hdc = GetDC(hwnd);
                int dx = 0;
                int dy = 0;
                if (_context.Horizontal)
                    dx = oldPosition - _position;
                else
                    dy = oldPosition - _position;
                Rectangle scrollRect = _bounds;
                Rectangle clipRect = _bounds;
                Rectangle updateRect = Rectangle.Empty;
                using (Region invalidRegion = new Region())
                {
                    if (ScrollDC(hdc, dx, dy, ref scrollRect, ref clipRect, (int)invalidRegion.GetHrgn(Graphics.FromHdc((IntPtr)hdc)), ref updateRect) != 0)
                    {
                        // Repaint the parts that were revealed.
                        _context.Invalidate(updateRect);
                        UpdateWindow(hwnd);
                    }
                }
            }
        }

        #endregion

        #region Child renderers

        protected override IEnumerable<Renderer> GetChildRenderers()
        {
            return _context.GetChildRenderers();
        }

        #endregion

        #region Translation functions

        private int GetCoordinate(Point p)
        {
            return _context.Horizontal ? p.X : p.Y;
        }

        private Point TranslateMouse(Point p)
        {
            if (_context.Horizontal)
                p.Offset(_position, 0);
            else
                p.Offset(0, _position);
            return p;
        }

        private void TranslateGraphics(Graphics graphics)
        {
            if (_context.Horizontal)
                graphics.TranslateTransform(-_position, 0);
            else
                graphics.TranslateTransform(0, -_position);
        }

        public Rectangle TranslateRectangle(Rectangle r)
        {
            if (_context.Horizontal)
                r.Offset(-_position, 0);
            else
                r.Offset(0, -_position);
            return r;
        }

        public void EnsureVisible(Rectangle rectangle)
        {
            // Calculate the range in which the rectangle is visible.
            int min = 0;
            int max = 0;
            if (_context.Horizontal)
            {
                min = rectangle.Right - _context.Bounds.Width;
                max = rectangle.Left;
            }
            else
            {
                min = rectangle.Bottom - _context.Bounds.Height;
                max = rectangle.Top;
            }

            // Determine whether the scroller needs to move.
            _depPaint.OnGet();
            int target = Math.Max(Math.Min(_position, max), min);
            if (target != _position)
            {
                _glideScroller.Position = target;
            }
        }

        #endregion
    }
}
