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
using System.Drawing;
using System.Diagnostics;

namespace UpdateControls.Themes.Solid
{
    public class SolidShapeTab : SolidShape
    {
        private enum Direction
        {
            North,
            East,
            South,
            West
        };

        public static SolidShape NorthInstance = new SolidShapeTab(Direction.North);
        public static SolidShape EastInstance = new SolidShapeTab(Direction.East);
        public static SolidShape SouthInstance = new SolidShapeTab(Direction.South);
        public static SolidShape WestInstance = new SolidShapeTab(Direction.West);

        private Direction _direction;

        private SolidShapeTab(Direction direction)
        {
            _direction = direction;
        }

        public override bool Normal(PointF bounds, PointF p, out PointF3D n, out float z, out float alpha)
        {
            const float c3 = -24.59413541f;
            const float c2 = 56.68827083f;
            const float c1 = -34.09413541f;
            // f(x) = c3 x^12 + c2 x^10 + c1 x^8 + 1
            // z = sqrt(1 - ((y+1)/(f(x)+1))^2))
            // Where -1 <= x <= 1 and 0 <= y <= 1
            // 0 <= z <= 1

            // f'(x) = 12c3 x^11 + 10c2 x^9 + 8c1 x^7
            // dz/dx = y^2/zf^3(x) f'(x)
            // dz/dy = -y/zf^2(x)

            if ((p.X >= 0.0f) && (p.X <= bounds.X) &&
                (p.Y >= 0.0f) && (p.Y <= bounds.Y))
            {
                // Convert to scaled units.
                float x, y, dxdpx, dydpy;
                if (_direction == Direction.North || _direction == Direction.South)
                {
                    // If the bounds are wide, then split down the middle.
                    if (bounds.X > bounds.Y)
                    {
                        float half = bounds.Y * 0.5f;
                        if (p.X < half)
                        {
                            dxdpx = 2.0f / bounds.Y;
                            x = dxdpx * p.X - 1.0f;
                        }
                        else if (p.X < bounds.X - half)
                        {
                            dxdpx = 0.0f;
                            x = 0.0f;
                        }
                        else
                        {
                            dxdpx = 2.0f / bounds.Y;
                            x = dxdpx * (p.X - bounds.X) + 1.0f;
                        }
                    }
                    else
                    {
                        dxdpx = 2.0f / bounds.X;
                        x = dxdpx * p.X - 1.0f;
                    }
                    if (_direction == Direction.North)
                    {
                        dydpy = -2.0f / bounds.Y;
                        y = dydpy * (p.Y+1) + 1.0f;
                    }
                    else
                    {
                        dydpy = 2.0f / bounds.Y;
                        y = dydpy * p.Y - 1.0f;
                    }
                }
                else
                {
                    // If the bounds are tall, then split down the middle.
                    if (bounds.Y > bounds.X)
                    {
                        float half = bounds.X * 0.5f;
                        if (p.Y < half)
                        {
                            dxdpx = 2.0f / bounds.X;
                            x = dxdpx * p.Y - 1.0f;
                        }
                        else if (p.Y < bounds.Y - half)
                        {
                            dxdpx = 0.0f;
                            x = 0.0f;
                        }
                        else
                        {
                            dxdpx = 2.0f / bounds.X;
                            x = dxdpx * (p.Y - bounds.Y) + 1.0f;
                        }
                    }
                    else
                    {
                        dxdpx = 2.0f / bounds.Y;
                        x = dxdpx * p.Y - 1.0f;
                    }
                    if (_direction == Direction.West)
                    {
                        dydpy = -2.0f / bounds.X;
                        y = dydpy * (p.X+1) + 1.0f;
                    }
                    else
                    {
                        dydpy = 2.0f / bounds.X;
                        y = dydpy * p.X - 1.0f;
                    }
                }

                float x2 = x * x;
                float x4 = x2 * x2;
                float x8 = x4 * x4;
                float fx = ((c3 * x2 + c2) * x2 + c1) * x8 + 1.0f;
                if (y <= fx)
                {
                    float yp1ofxp1 = (y+1.0f)/(fx+1.0f);
                    float yp1ofxp12 = yp1ofxp1*yp1ofxp1;
                    z = 1.0f - yp1ofxp12;

                    float fpx = ((12.0f * c3 * x2 + 10.0f * c2) * x2 + 8.0f * c1) * x4 * x2 * x;
                    float dzdx = 2.0f * yp1ofxp12 / (fx + 1.0f) * fpx;
                    float dzdy = -2.0f * yp1ofxp1 / (fx + 1.0f);

                    // Calculate the normal.
                    if (_direction == Direction.North || _direction == Direction.South)
                    {
                        // n || (-dz/dpx, -dz/dpy, 1)
                        // dz/dpx = dz/dx dx/dpx
                        // dz/dpy = dz/dy dy/dpy
                        n = new PointF3D(
                            -dzdx * dxdpx,
                            -dzdy * dydpy,
                            1.0f);
                    }
                    else
                    {
                        // n || (-dz/dpy, -dz/dpx, 1)
                        // dz/dpx = dz/dx dx/dpx
                        // dz/dpy = dz/dy dy/dpy
                        n = new PointF3D(
                            -dzdy * dydpy,
                            -dzdx * dxdpx,
                            1.0f);
                    }

                    double dgdpx = dxdpx / dydpy * fpx;
                    alpha = Math.Abs((fx - y) / dydpy) / (float)Math.Sqrt(dgdpx * dgdpx + 1.0);
                    if (alpha > 1.0f)
                        alpha = 1.0f;
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
