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
    public class AlignmentFlags
    {
        public enum HorizontalAlignment
        {
            Left,
            Center,
            Right
        }

        public enum VerticalAlignment
        {
            Top,
            Middle,
            Bottom
        }

        private HorizontalAlignment _horizontal;
        private VerticalAlignment _vertical;

        public AlignmentFlags(ContentAlignment alignment)
        {
            if (alignment == ContentAlignment.TopLeft)
            {
                _horizontal = HorizontalAlignment.Left;
                _vertical = VerticalAlignment.Top;
            }
            else if (alignment == ContentAlignment.MiddleLeft)
            {
                _horizontal = HorizontalAlignment.Left;
                _vertical = VerticalAlignment.Middle;
            }
            else if (alignment == ContentAlignment.BottomLeft)
            {
                _horizontal = HorizontalAlignment.Left;
                _vertical = VerticalAlignment.Bottom;
            }
            else if (alignment == ContentAlignment.TopCenter)
            {
                _horizontal = HorizontalAlignment.Center;
                _vertical = VerticalAlignment.Top;
            }
            else if (alignment == ContentAlignment.MiddleCenter)
            {
                _horizontal = HorizontalAlignment.Center;
                _vertical = VerticalAlignment.Middle;
            }
            else if (alignment == ContentAlignment.BottomCenter)
            {
                _horizontal = HorizontalAlignment.Center;
                _vertical = VerticalAlignment.Bottom;
            }
            else if (alignment == ContentAlignment.TopRight)
            {
                _horizontal = HorizontalAlignment.Right;
                _vertical = VerticalAlignment.Top;
            }
            else if (alignment == ContentAlignment.MiddleRight)
            {
                _horizontal = HorizontalAlignment.Right;
                _vertical = VerticalAlignment.Middle;
            }
            else // if (alignment == ContentAlignment.BottomRight)
            {
                _horizontal = HorizontalAlignment.Right;
                _vertical = VerticalAlignment.Bottom;
            }
        }

        private AlignmentFlags(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            _horizontal = horizontal;
            _vertical = vertical;
        }

        public HorizontalAlignment Horizontal
        {
            get { return _horizontal; }
        }

        public VerticalAlignment Vertical
        {
            get { return _vertical; }
        }

        public AlignmentFlags Inverse
        {
            get
            {
                return new AlignmentFlags(
                    (_horizontal == HorizontalAlignment.Left) ? HorizontalAlignment.Right :
                    (_horizontal == HorizontalAlignment.Center) ? HorizontalAlignment.Center :
                    /*(_horizontal == HorizontalAlignment.Right) ?*/ HorizontalAlignment.Left,
                    (_vertical == VerticalAlignment.Top) ? VerticalAlignment.Bottom :
                    (_vertical == VerticalAlignment.Middle) ? VerticalAlignment.Middle :
                    /*(_vertical == VerticalAlignment.Bottom) ?*/ VerticalAlignment.Top);
            }
        }

        public TextFormatFlags TextFormatFlags
        {
            get
            {
                return
                    ((_horizontal == HorizontalAlignment.Left) ? TextFormatFlags.Left :
                     (_horizontal == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                    /*(_horizontal == HorizontalAlignment.Right) ?*/ TextFormatFlags.Right) |
                    ((_vertical == VerticalAlignment.Top) ? TextFormatFlags.Top :
                     (_vertical == VerticalAlignment.Middle) ? TextFormatFlags.VerticalCenter :
                    /*(_vertical == VerticalAlignment.Bottom) ?*/ TextFormatFlags.Bottom);
            }
        }

        public Rectangle AlignRectangle(Rectangle bounds, Size size)
        {
            Rectangle result = new Rectangle();

            if (_horizontal == HorizontalAlignment.Left)
                result.X = bounds.Left;
            else if (_horizontal == HorizontalAlignment.Center)
                result.X = bounds.Left + (bounds.Width - size.Width) / 2;
            else //if (_horizontal == HorizontalAlignment.Right)
                result.X = bounds.Right - size.Width;

            if (_vertical == VerticalAlignment.Top)
                result.Y = bounds.Top;
            else if (_vertical == VerticalAlignment.Middle)
                result.Y = bounds.Top + (bounds.Height - size.Height) / 2;
            else //if (_vertical == VerticalAlignment.Bottom)
                result.Y = bounds.Bottom - size.Height;

            result.Size = size;
            return result;
        }
    }
}
