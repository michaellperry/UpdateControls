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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace UpdateControls.Themes.Renderers
{
    public class Renderer : IDisposable
    {
        private List<Renderer> _childRenderers = new List<Renderer>();
        private Dependent _depChildRenderers;

        public Renderer()
        {
            _depChildRenderers = new Dependent(delegate()
            {
                // Recycle the renderers.
                using (var bin = _childRenderers.Recycle())
                {
                    _childRenderers.AddRange(
                        from r in GetChildRenderers()
                        select bin.Extract(r));
                }
            });
        }

        public virtual void Dispose()
        {
            _depChildRenderers.Dispose();
            foreach (Renderer renderer in _childRenderers)
                renderer.Dispose();
        }

        public virtual IEnumerable<Renderer> ChildRenderers
        {
            get
            {
                _depChildRenderers.OnGet();
                return _childRenderers;
            }
        }

        public virtual IEnumerable<Renderer> BottomUp
        {
            get
            {
                _depChildRenderers.OnGet();
                foreach (Renderer renderer in _childRenderers)
                {
                    yield return renderer;
                    foreach (Renderer child in renderer.BottomUp)
                    {
                        yield return child;
                    }
                }
            }
        }

        public virtual IEnumerable<Renderer> TopDown
        {
            get
            {
                _depChildRenderers.OnGet();
                for ( int index = _childRenderers.Count-1; index >= 0; --index)
                {
                    Renderer renderer = _childRenderers[index];
                    foreach (Renderer child in renderer.TopDown)
                    {
                        yield return child;
                    }
                    yield return renderer;
                }
            }
        }

        public virtual void OnIdle()
        {
            _depChildRenderers.OnGet();
        }
        public virtual bool HitTest(Point mouse) { return false; }
        public virtual void OnMouseEnter() {}
        public virtual void OnMouseLeave() {}
        public virtual void OnMouseMove(Point mouse) { }
        public virtual void OnMouseDown(Point mouse, MouseButtons button, int clicks, Keys modifierKeys) { }
        public virtual void OnMouseDrag(Point start, Point mouse) {}
        public virtual void OnMouseDragTimer() { }
        public virtual void OnMouseUp(Point start, Point mouse) { }
        public virtual void PaintBackground(Graphics graphics) { }
        public virtual void PaintForeground(Graphics graphics) { }

        protected virtual IEnumerable<Renderer> GetChildRenderers() { yield break; }
    }
}
