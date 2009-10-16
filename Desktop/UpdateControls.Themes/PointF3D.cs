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

namespace UpdateControls.Themes
{
    public struct PointF3D
    {
        private float _x;
        private float _y;
        private float _z;

        public PointF3D(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public float Dot(PointF3D b)
        {
            return _x * b._x + _y * b._y + _z * b._z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(this.Dot(this));
        }

        public void Normalize()
        {
            float length = Length();
            if (Math.Abs(length) > 0.001f)
            {
                _x /= length;
                _y /= length;
                _z /= length;
            }
        }

        public bool IsValid
        {
            get
            {
                return
                    !float.IsNaN(_x) &&
                    !float.IsNaN(_y) &&
                    !float.IsNaN(_z);
            }
        }

        public static PointF3D Zero = new PointF3D(0.0f, 0.0f, 0.0f);

        public PointF3D Times(float scalar)
        {
            return new PointF3D(_x * scalar, _y * scalar, _z * scalar);
        }

        public PointF3D Plus(PointF3D b)
        {
            return new PointF3D(_x + b._x, _y + b._y, _z + b._z);
        }
    }
}
