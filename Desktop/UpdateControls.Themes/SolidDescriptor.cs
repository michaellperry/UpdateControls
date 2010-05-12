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
using System.Runtime.InteropServices;

namespace UpdateControls.Themes
{
    [Serializable]
    public struct SolidDescriptor
    {
        private PointF _direction;
        private float _ambience;
        private float _aperature;

        private Color _color;
        private float _thickness;
        private float _refraction;
        private float _sharpness;
        private bool _transparent;

        public SolidDescriptor(
            PointF direction,
            float ambience,
            float aperature,
            Color color,
            float thickness,
            float refraction,
            float sharpness,
            bool transparent)
        {
            _direction = direction;
            _ambience = ambience;
            _aperature = aperature;
            _color = color;
            _thickness = thickness;
            _refraction = refraction;
            _sharpness = sharpness;
            _transparent = transparent;
        }

        public PointF Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public float Ambience
        {
            get { return _ambience; }
            set { _ambience = value; }
        }

        public float Aperature
        {
            get { return _aperature; }
            set { _aperature = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public float Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        public float Refraction
        {
            get { return _refraction; }
            set { _refraction = value; }
        }

        public float Sharpness
        {
            get { return _sharpness; }
            set { _sharpness = value; }
        }

        public bool Transparent
        {
            get { return _transparent; }
            set { _transparent = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SolidDescriptor))
                return false;
            SolidDescriptor that = (SolidDescriptor)obj;

            return
                _direction.Equals(that._direction) &&
                _ambience.Equals(that._ambience) &&
                _aperature.Equals(that._aperature) &&
                _color.Equals(that._color) &&
                _thickness.Equals(that._thickness) &&
                _refraction.Equals(that._refraction) &&
                _sharpness.Equals(that._sharpness) &&
                _transparent.Equals(that._transparent);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash = hash * 37 + _direction.GetHashCode();
            hash = hash * 37 + _ambience.GetHashCode();
            hash = hash * 37 + _aperature.GetHashCode();
            hash = hash * 37 + _color.GetHashCode();
            hash = hash * 37 + _thickness.GetHashCode();
            hash = hash * 37 + _refraction.GetHashCode();
            hash = hash * 37 + _sharpness.GetHashCode();
            hash = hash * 37 + _transparent.GetHashCode();
            return hash;
        }

        public static bool operator ==(SolidDescriptor a, SolidDescriptor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SolidDescriptor a, SolidDescriptor b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return string.Format("new SolidDescriptor(new PointF({0}f, {1}f), {2}f, {3}f, Color.FromArgb({4}, {5}, {6}), {7}f, {8}f, {9}f, {10})",
                _direction.X, _direction.Y,
                _ambience,
                _aperature,
                _color.R, _color.G, _color.B,
                _thickness,
                _refraction,
                _sharpness,
                _transparent);
        }
    }
}
