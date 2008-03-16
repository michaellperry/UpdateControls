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

namespace UpdateControls.Themes.Solid
{
    public class SolidShapeTile : SolidShape
    {
        public static SolidShape Instance = new SolidShapeTile();

        private SolidShapeTile()
        {
        }

        public override bool Normal(PointF bounds, PointF p, out PointF3D n, out float z, out float alpha)
        {
            // z = (1-x^2)(1-y^2)
            // Where -1 <= x <= 1 and -1 <= y <= 1
            // 0 <= z <= 1

            // dz/dx = -2x(1-y^2)
            // dz/dy = -2y(1-x^2)

            if ((p.X >= 0.0f) && (p.X <= bounds.X) &&
                (p.Y >= 0.0f) && (p.Y <= bounds.Y))
            {
                // Convert to scaled units.
                // dx/dpx = 2/bx
                // dy/dpy = 2/by
                float x = 2.0f * p.X / bounds.X - 1.0f;
                float y = 2.0f * p.Y / bounds.Y - 1.0f;

                float omx2 = 1.0f - x * x;
                float omy2 = 1.0f - y * y;
                z = omx2 * omy2;

                // Calculate the normal.
                // n || (-dz/dpx, -dz/dpy, 1)
                // dz/dpx = dz/dx dx/dpx
                // dz/dpy = dz/dy dy/dpy
                n = new PointF3D(
                    4.0f * x * omy2 / bounds.X,
                    4.0f * y * omx2 / bounds.Y,
                    1.0f);

                alpha = 1.0f;
                return true;
            }
            else
            {
                n = PointF3D.Zero;
                z = 0.0f;
                alpha = 0.0f;
                return false;
            }
        }
    }
}
