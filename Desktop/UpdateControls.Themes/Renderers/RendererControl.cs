/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UpdateControls.Themes.Renderers
{
    public abstract class RendererControl : Control
    {
        private RendererController _controller;
        private Timer _timer;
        private bool _initialized = false;

        public RendererControl()
        {
            _controller = new RendererController(new RendererController.GetRenderersDelegate(GetRenderers));
            _timer = new Timer();
            _timer.Tick += new EventHandler(OnTimer);
            _timer.Interval = 250;
        }

        protected abstract IEnumerable<Renderer> GetRenderers();
        protected virtual void OnIdle()
        {
            _controller.OnIdle();
            _initialized = true;
        }
        protected virtual void OnDestroy() { }
        protected override void OnEnabledChanged(EventArgs e)
        {
            // Don't give the base class a chance to repaint.
            // base.OnEnabledChanged(e);
        }

        /// <summary>
        /// Register idle-time updates for the control.
        /// </summary>
        /// <param name="e">unused</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);
            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Unregister idle-time updates for the control.
        /// </summary>
        /// <param name="e">unused</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            _controller.Dispose();
            OnDestroy();
            base.OnHandleDestroyed(e);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            OnIdle();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _controller.OnMouseLeave();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _controller.OnMouseMove(e.Location);
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_controller.OnMouseDown(e.Location, e.Button, e.Clicks, ModifierKeys))
            {
                this.Capture = true;
                _timer.Start();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_controller.OnMouseUp(e.Location))
            {
                this.Capture = false;
                _timer.Stop();
            }
            base.OnMouseUp(e);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            _controller.OnTimer();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // We don't receive idle messages while the user is resizing.
            if (_initialized)
                OnIdle();
            base.OnSizeChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            base.OnPaintBackground(pe);
            // Bring everything up to date before you start painting.
            OnIdle();
            _controller.PaintBackground(pe.Graphics);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Bring everything up to date before you start painting.
            OnIdle();
            _controller.PaintForeground(pe.Graphics);
        }

    }
}
