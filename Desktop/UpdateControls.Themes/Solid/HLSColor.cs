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

namespace UpdateControls.Themes.Solid
{
    public struct HLSColor
    {
        private float _h;
        private float _l;
        private float _s;

        public HLSColor(Color c)
        {
            float r = c.R / 255.0f;
            float g = c.G / 255.0f;
            float b = c.B / 255.0f;
            float maximum = Math.Max(r, Math.Max(g, b));
            float minimum = Math.Min(r, Math.Min(g, b));
            float delta = maximum - minimum;

            _l = 0.5f * (maximum + minimum);
            if (delta <= 0.001)
            {
                _s = 0.0f;
                _h = 0.0f;
            }
            else
            {
                if (_l <= 0.5)
                    _s = (maximum - minimum) / (maximum + minimum);
                else
                    _s = (maximum - minimum) / (2.0f - maximum + minimum);

                delta = maximum - minimum;
                if (r == maximum)
                    _h = (g - b) / delta;
                else if (g == maximum)
                    _h = 2.0f + (b - r) / delta;
                else //if (b = maximum)
                    _h = 4.0f + (r - g) / delta;
                _h = _h * 60.0f;
                if (_h < 0.0f)
                    _h = _h + 360.0f;
            }
        }

        public float H
        {
            get { return _h; }
            set { _h = value; }
        }

        public float L
        {
            get { return _l; }
            set { _l = value; }
        }

        public float S
        {
            get { return _s; }
            set { _s = value; }
        }

        public Color RGB
        {
            get
            {
                float m2;
                if (_l <= 0.5f)
                    m2 = _l * (1.0f + _s);
                else
                    m2 = _l + _s - _l * _s;

                float m1 = 2.0f * _l - m2;

                return Color.FromArgb(
                  (int)(255.9f * Value(m1, m2, _h + 120.0f)),
                  (int)(255.9f * Value(m1, m2, _h)),
                  (int)(255.9f * Value(m1, m2, _h - 120.0f)));
            }
        }

        private float Value(float n1, float n2, float hue)
        {
            float result;

            if (hue > 360.0f)
                hue = hue - 360.0f;
            else if (hue < 0.0f)
                hue = hue + 360.0f;

            if (hue < 60.0f)
                result = n1 + (n2 - n1) * hue / 60.0f;
            else if (hue < 180.0f)
                result = n2;
            else if (hue < 240.0f)
                result = n1 + (n2 - n1) * (240.0f - hue) / 60.0f;
            else
                result = n1;

            if (result > 1.0f)
                result = 1.0f;
            return result;
        }
    }
}
