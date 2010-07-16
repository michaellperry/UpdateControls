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
using System.Drawing.Drawing2D;

namespace UpdateControls.Themes.GraphicsUtil
{
    public class GraphicsStateSaver : IDisposable
    {
        private Graphics _graphics;
        private GraphicsState _previousState;

        public GraphicsStateSaver(Graphics graphics)
        {
            _graphics = graphics;
            _previousState = graphics.Save();
        }

        public void Dispose()
        {
            _graphics.Restore(_previousState);
        }
    }
}
