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

namespace UpdateControls.Themes.Renderers
{
    public class RendererController : IDisposable
    {
        public delegate IEnumerable<Renderer> GetRenderersDelegate();

        private List<Renderer> _renderers = new List<Renderer>();
        private Dependent _depRenderers;

        private Renderer _hit = null;
        private Point _start = Point.Empty;
        private Point _end = Point.Empty;
        private bool _down = false;

        public RendererController(GetRenderersDelegate getRenderers)
        {
            _depRenderers = new Dependent(delegate()
            {
                // Recycle the renderers.
                using (RecycleBin<Renderer> bin = new RecycleBin<Renderer>(_renderers))
                {
                    _renderers.Clear();
                    foreach (Renderer renderer in getRenderers())
                    {
                        _renderers.Add(bin.Extract(renderer));
                    }
                }
            });
        }

        public void Dispose()
        {
            _depRenderers.Dispose();
            foreach (Renderer renderer in _renderers)
                renderer.Dispose();
        }

        private IEnumerable<Renderer> BottomUp
        {
            get
            {
                _depRenderers.OnGet();
                foreach (Renderer renderer in _renderers)
                {
                    yield return renderer;
                    foreach (Renderer child in renderer.BottomUp)
                    {
                        yield return child;
                    }
                }
            }
        }

        private IEnumerable<Renderer> TopDown
        {
            get
            {
                _depRenderers.OnGet();
                for ( int index = _renderers.Count-1; index >= 0; --index)
                {
                    Renderer renderer = _renderers[index];
                    foreach (Renderer child in renderer.TopDown)
                    {
                        yield return child;
                    }
                    yield return renderer;
                }
            }
        }

        private Renderer Hit(Point mouse)
        {
            foreach (Renderer renderer in TopDown)
                if (renderer.HitTest(mouse))
                    return renderer;
            return null;
        }

        private void SwitchHit(Point mouse)
        {
            Renderer nextHit = Hit(mouse);
            if (_hit != nextHit)
            {
                if (_hit != null)
                    _hit.OnMouseLeave();
                _hit = nextHit;
                if (_hit != null)
                    _hit.OnMouseEnter();
            }
            if (!_down && _hit != null)
                _hit.OnMouseMove(mouse);
        }

        public void OnIdle()
        {
            foreach (Renderer renderer in BottomUp)
                renderer.OnIdle();
        }

        public void OnMouseLeave()
        {
            if (!_down && _hit != null)
            {
                _hit.OnMouseLeave();
                _hit = null;
            }
        }

        public bool OnMouseDown(Point mouse, MouseButtons button, int clicks, Keys modifierKeys)
        {
            _down = true;
            _start = mouse;
            _end = mouse;

            SwitchHit(mouse);
            if (_hit != null)
                _hit.OnMouseDown(_start, button, clicks, modifierKeys);
            return _hit != null;
        }

        public void OnMouseMove(Point mouse)
        {
            if (_down)
            {
                if (_hit != null)
                {
                    _end = mouse;
                    _hit.OnMouseDrag(_start, _end);
                }
            }
            else
            {
                SwitchHit(mouse);
            }
        }

        public bool OnMouseUp(Point mouse)
        {
            bool wasCapture = _down && _hit != null;
            if (_down)
            {
                _down = false;
                if (_hit != null)
                    _hit.OnMouseUp(_start, mouse);
                SwitchHit(mouse);
            }
            return wasCapture;
        }

        public void OnTimer()
        {
            if (_down && _hit != null)
            {
                _hit.OnMouseDragTimer();
                _hit.OnMouseDrag(_start, _end);
            }
        }

        public void PaintBackground(Graphics graphics)
        {
            foreach (Renderer renderer in BottomUp)
                renderer.PaintBackground(graphics);
        }

        public void PaintForeground(Graphics graphics)
        {
            foreach (Renderer renderer in BottomUp)
                renderer.PaintForeground(graphics);
        }
    }
}
