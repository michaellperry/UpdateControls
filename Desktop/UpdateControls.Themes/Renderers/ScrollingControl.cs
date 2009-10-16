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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UpdateControls.Themes.Renderers
{
    public abstract class ScrollingControl : Control
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        private const int WS_HSCROLL = 0x00100000;
        private const int WS_VSCROLL = 0x00200000;

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        private const int SIF_RANGE = 0x0001;
        private const int SIF_PAGE = 0x0002;
        private const int SIF_POS = 0x0004;
        private const int SIF_DISABLENOSCROLL = 0x0008;
        private const int SIF_TRACKPOS = 0x0010;
        private const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

        private const int GWL_WNDPROC = -4;
        private const int GWL_HINSTANCE = -6;
        private const int GWL_HWNDPARENT = -8;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_USERDATA = -21;
        private const int GWL_ID = -12;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOREDRAW = 0x0008;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int SWP_HIDEWINDOW = 0x0080;
        private const int SWP_NOCOPYBITS = 0x0100;
        private const int SWP_NOOWNERZORDER = 0x0200;
        private const int SWP_NOSENDCHANGING = 0x0400;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        [DllImport("user32")]
        private static extern int SetScrollInfo(IntPtr hwnd, int bar, ref SCROLLINFO lpcScrollInfo, bool redraw);
        [DllImport("user32")]
        private static extern int GetScrollInfo(IntPtr hwnd, int bar, ref SCROLLINFO lpcScrollInfo);
        [DllImport("user32")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);
        [DllImport("user32")]
        public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        public delegate int GetSize();

        [Serializable]
        public class ScrollInfo
        {
            private bool _enabled = false;
            private int _thumbPosition = 0;
            private int _rowSize = 0;

            private GetSize _getPageSize;
            private GetSize _getWindowSize;
            private Independent _dynEnabled = new Independent();
            private Independent _dynInfo = new Independent();

            public ScrollInfo(GetSize getPageSize, GetSize getWindowSize)
            {
                _getPageSize = getPageSize;
                _getWindowSize = getWindowSize;
            }

            public bool Enabled
            {
                get { _dynEnabled.OnGet(); return _enabled; }
                set { _dynEnabled.OnSet(); _enabled = value; }
            }

            public int WindowSize
            {
                get { return _getWindowSize(); }
            }

            public int ThumbPosition
            {
                get { _dynInfo.OnGet(); return _thumbPosition; }
                set { _dynInfo.OnSet(); _thumbPosition = value; }
            }

            public int PageSize
            {
                get { return _getPageSize(); }
            }

            public int RowSize
            {
                get { return _rowSize; }
                set { _rowSize = value; }
            }

            internal SCROLLINFO ToWin32()
            {
                _dynInfo.OnGet();
                SCROLLINFO scrollInfo = new SCROLLINFO();
                scrollInfo.cbSize = Marshal.SizeOf(scrollInfo);
                scrollInfo.fMask = SIF_ALL | SIF_DISABLENOSCROLL;
                scrollInfo.nMin = 0;
                scrollInfo.nMax = WindowSize - 1;
                scrollInfo.nPage = PageSize;
                scrollInfo.nPos = _thumbPosition;
                scrollInfo.nTrackPos = _thumbPosition;
                return scrollInfo;
            }
        }

        private bool _initialized = false;
        private ScrollInfo _horizontal;
        private ScrollInfo _vertical;

        private Dependent _depHorizontal;
        private Dependent _depVertical;
        private Dependent _depStyle;

        public ScrollingControl()
        {
            _horizontal = new ScrollInfo(GetHorizontalPageSize, GetHorizontalWindowSize);
            _vertical = new ScrollInfo(GetVerticalPageSize, GetVerticalWindowSize);

            _depHorizontal = new Dependent(UpdateHorizontal);
            _depVertical = new Dependent(UpdateVertical);
            _depStyle = new Dependent(UpdateStyle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL)
            {
                ScrollInfo info = (m.Msg == WM_HSCROLL) ? _horizontal : _vertical;

                uint wParam = (uint)m.WParam.ToInt32();
                ScrollEventType scrollEvent = GetEventType(wParam & 0xffff);

                int oldValue = info.ThumbPosition;
                int newValue = 0;
                if (scrollEvent == ScrollEventType.SmallDecrement)
                    newValue = oldValue - info.RowSize;
                else if (scrollEvent == ScrollEventType.SmallIncrement)
                    newValue = oldValue + info.RowSize;
                else if (scrollEvent == ScrollEventType.LargeDecrement)
                    newValue = oldValue - info.PageSize;
                else if (scrollEvent == ScrollEventType.LargeIncrement)
                    newValue = oldValue + info.PageSize;
                else if (scrollEvent == ScrollEventType.ThumbPosition || scrollEvent == ScrollEventType.ThumbTrack)
                    newValue = (int)(wParam >> 16);
                else if (scrollEvent == ScrollEventType.First)
                    newValue = 0;
                else if (scrollEvent == ScrollEventType.Last)
                    newValue = info.WindowSize - info.PageSize;
                else
                    newValue = oldValue;

                if (newValue != oldValue)
                {
                    // Set the new position.
                    if (newValue < 0)
                        info.ThumbPosition = 0;
                    else if (newValue > info.WindowSize - info.PageSize)
                        info.ThumbPosition = info.WindowSize - info.PageSize;
                    else
                        info.ThumbPosition = newValue;
                    if (m.Msg == WM_HSCROLL)
                    {
                        ScrollHorizontal(newValue - oldValue);
                        _depHorizontal.OnGet();
                    }
                    else
                    {
                        ScrollVertical(newValue - oldValue);
                        _depVertical.OnGet();
                    }
                }
            }
            base.WndProc(ref m);
        }

        // Based on SB_* constants
        private static ScrollEventType[] _events =
            new ScrollEventType[] {
									  ScrollEventType.SmallDecrement,
									  ScrollEventType.SmallIncrement,
									  ScrollEventType.LargeDecrement,
									  ScrollEventType.LargeIncrement,
									  ScrollEventType.ThumbPosition,
									  ScrollEventType.ThumbTrack,
									  ScrollEventType.First,
									  ScrollEventType.Last,
									  ScrollEventType.EndScroll
								  };
        /// <summary>
        /// Decode the type of scroll message
        /// </summary>
        /// <param name="wParam">Lower word of scroll notification</param>
        /// <returns></returns>
        private ScrollEventType GetEventType(uint wParam)
        {
            if (wParam < _events.Length)
                return _events[wParam];
            else
                return ScrollEventType.EndScroll;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Vertical.Enabled)
            {
                int oldValue = Vertical.ThumbPosition;
                int newValue = oldValue - e.Delta / 120;
                if (newValue != oldValue)
                {
                    // Set the new position.
                    if (newValue < 0)
                        Vertical.ThumbPosition = 0;
                    else if (newValue > Vertical.WindowSize - Vertical.PageSize)
                        Vertical.ThumbPosition = Vertical.WindowSize - Vertical.PageSize;
                    else
                        Vertical.ThumbPosition = newValue;

                    ScrollVertical(newValue - oldValue);
                    _depVertical.OnGet();
                }
            }

            base.OnMouseWheel(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.Style = ModifyStyle(createParams.Style);
                return createParams;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            _depStyle.Dispose();
            _depHorizontal.Dispose();
            _depVertical.Dispose();
            base.OnHandleDestroyed(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (_initialized)
                OnIdle();
            base.OnSizeChanged(e);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            _initialized = true;
            OnIdle();
        }

        private void OnIdle()
        {
            _depStyle.OnGet();
            _depHorizontal.OnGet();
            _depVertical.OnGet();
        }

        private void UpdateHorizontal()
        {
            if (Horizontal.Enabled)
            {
                SCROLLINFO scrollInfo = Horizontal.ToWin32();
                SetScrollInfo(Handle, SB_HORZ, ref scrollInfo, true);
            }
        }

        private void UpdateVertical()
        {
            if (Vertical.Enabled)
            {
                SCROLLINFO scrollInfo = Vertical.ToWin32();
                SetScrollInfo(Handle, SB_VERT, ref scrollInfo, true);
            }
        }

        private void UpdateStyle()
        {
            // Get the current window style.
            int oldStyle = GetWindowLong(Handle, GWL_STYLE);
            int newStyle = ModifyStyle(oldStyle);
            // Set the new style.
            if (newStyle != oldStyle)
            {
                SetWindowLong(Handle, GWL_STYLE, newStyle);
                SetWindowPos(Handle, 0, Location.X, Location.Y, Size.Width, Size.Height, SWP_FRAMECHANGED);
            }
        }

        private int ModifyStyle(int style)
        {
            // Mask out the scroll bars.
            style &= ~(WS_HSCROLL | WS_VSCROLL);
            // Add the selected scroll bar styles.
            if (Horizontal.Enabled)
                style |= WS_HSCROLL;
            if (Vertical.Enabled)
                style |= WS_VSCROLL;
            return style;
        }

        protected ScrollInfo Horizontal
        {
            get { return _horizontal; }
        }

        protected ScrollInfo Vertical
        {
            get { return _vertical; }
        }

        protected virtual void ScrollVertical(int offset) { }
        protected virtual void ScrollHorizontal(int offset) { }
        protected virtual int GetVerticalPageSize() { return 0; }
        protected virtual int GetHorizontalPageSize() { return 0; }
        protected virtual int GetVerticalWindowSize() { return 0; }
        protected virtual int GetHorizontalWindowSize() { return 0; }
    }
}
