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

namespace UpdateControls.Themes.Inertia
{
    public class GlideScroller : IDisposable
    {
        public interface Context
        {
            int Range { get; }
            int Width { get; }
        }
        private Context _context;

        // Dynamic state.
        private bool _inside = false;   // True when the mouse is in the scroll region.
        private int _enter = 0;         // The point on the x axis when the mouse entered.
        private int _origin = 0;        // The point on the y axis when the mouse entered.
        private int _mouse = 0;         // The point on the x axis of the mouse.

        // Dependent input.
        private int _range = 0;         // The maximum on the y axis.
        private int _width = 0;         // The maximum on the x axis.

        private Independent _dynState = new Independent();
        private Dependent _depRange;
        private Dependent _depWidth;

        public GlideScroller(Context context)
        {
            _context = context;
            _depRange = new Dependent(delegate() { _range = _context.Range; });
            _depWidth = new Dependent(delegate() { _width = _context.Width; });
        }

        public void Dispose()
        {
            _depRange.Dispose();
            _depWidth.Dispose();
        }

        public int Position
        {
            get
            {
                // If there is nowhere to go, stay at zero.
                _depRange.OnGet();
                _depWidth.OnGet();
                if (_range <= _width)
                    return 0;

                _dynState.OnGet();
                if (_inside)
                {
                    // If the mouse is off the edge, return the extreme.
                    if (_mouse < 0)
                        return 0;
                    else if (_mouse > _width)
                        return _range - _width;

                    // Limit the inflection point to avoid backward scrolling.
                    int origin = Math.Min(_origin, _range - _width);

                    // If the mouse is to the left of the entry point,
                    // the line terminates at zero.
                    if (_mouse < _enter)
                        return origin * _mouse / _enter;
                    // If the mouse is to the right and the line is
                    // not vertical, the line terminates at the range.
                    else if (_enter < _width)
                        return (_range - _width - origin) * (_mouse - _enter) / (_width - _enter) + origin;
                    // Otherwise, just stay at the origin.
                    else return origin;
                }
                else
                {
                    // Stay at the point of departure.
                    return Math.Min(_origin, _range - _width);
                }
            }
            set
            {
                // Modify the parameters so that position becomes the desired value.
                _dynState.OnSet();
                _origin = value;
                if (_inside)
                    _enter = _mouse;
            }
        }

        public void Enter(int mouse)
        {
            _dynState.OnSet();
            if (!_inside)
            {
                // Start at the current position.
                _origin = Position;
                _enter = mouse;
            }
            _mouse = mouse;
            _inside = true;
        }

        public void Exit()
        {
            if (_inside)
            {
                _dynState.OnSet();
                _origin = Position;
                _inside = false;
            }
        }
    }
}
