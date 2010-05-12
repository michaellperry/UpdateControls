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
    public class SolidShapeRoundedRectangle : SolidShape
    {
        public static SolidShape Instance = new SolidShapeRoundedRectangle();

        private SolidShapeRoundedRectangle()
        {
        }

        public override bool Normal(PointF bounds, PointF p, out PointF3D n, out float z, out float alpha)
        {
            // z = 1 - x^2 - y^2
            // Where -1 <= x <= 1 and -1 <= y <= 1
            // 0 <= z <= 1

            // dz/dx = -2x
            // dz/dy = -2y

            if ((p.X >= 0.0f) && (p.X <= bounds.X) &&
                (p.Y >= 0.0f) && (p.Y <= bounds.Y) &&
                (bounds.X > 2.0f && bounds.Y > 2.0f))
            {
                // Round all corners based on the shorter dimension.
                float r = Math.Min(bounds.X, bounds.Y) * 0.25F;
                if (p.X > bounds.X - r)
                    p.X -= bounds.X - r - r;
                else if (p.X > r)
                    p.X = r;
                if (p.Y > bounds.Y - r)
                    p.Y -= bounds.Y - r - r;
                else if (p.Y > r)
                    p.Y = r;

                // Convert to scaled units.
                // dx/dpx = 2/bx
                // dy/dpy = 2/by
                float x = p.X / r - 1.0f;
                float y = p.Y / r - 1.0f;

                z = 1.0f - x * x - y * y;
                if (z > 0)
                {
                    // Calculate the normal.
                    // n || (-dz/dpx, -dz/dpy, 1)
                    // dz/dpx = dz/dx dx/dpx
                    // dz/dpy = dz/dy dy/dpy
                    n = new PointF3D(
                        2.0f * x / r,
                        2.0f * y / r,
                        1.0f);

                    // Calculate the distance from the edge.
                    float dx = p.X - r;
                    float dy = p.Y - r;
                    float d = r - (float)Math.Sqrt(dx * dx + dy * dy);
                    if (d > 1.0f)
                        alpha = 1.0f;
                    else if (d < 0.0f)
                        alpha = 0.0f;
                    else
                        alpha = d;
                    return true;
                }
            }

            n = PointF3D.Zero;
            z = 0.0f;
            alpha = 0.0f;
            return false;
        }
    }
}
