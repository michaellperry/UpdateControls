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

namespace UpdateControls.Themes.GraphicsUtil
{
    public static class GraphicsUtil
    {
        /// <summary>
        /// Rotate the rectangle clockwise so that it renders into a counter-clockwise graphics context.
        /// </summary>
        /// <param name="rectangle">The rectangle to rotate.</param>
        /// <returns>The rotated rectangle.</returns>
        public static Rectangle RotateRectangleCCW(Rectangle rectangle)
        {
            return new Rectangle(-rectangle.Bottom, rectangle.Left, rectangle.Height, rectangle.Width);
        }

        /// <summary>
        /// Rotate the rectangle counter-clockwise so that it renders into a clockwise graphics context.
        /// </summary>
        /// <param name="rectangle">The rectangle to rotate.</param>
        /// <returns>The rotated rectangle.</returns>
        public static Rectangle RotateRectangleCW(Rectangle rectangle)
        {
            return new Rectangle(rectangle.Top, -rectangle.Right, rectangle.Height, rectangle.Width);
        }

        /// <summary>
        /// Rotate the graphics context 90 degress clockwise.
        /// </summary>
        /// <param name="graphics">The graphics context to rotate.</param>
        public static void RotateGraphicsCW(Graphics graphics)
        {
            graphics.RotateTransform(90.0F);
        }

        /// <summary>
        /// Rotate the graphics context 90 degrees counter-clockwise.
        /// </summary>
        /// <param name="graphics">The graphics context to rotate.</param>
        public static void RotateGraphicsCCW(Graphics graphics)
        {
            graphics.RotateTransform(-90.0F);
        }
    }
}
