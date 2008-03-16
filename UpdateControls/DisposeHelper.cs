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

namespace UpdateControls
{
    /// <summary>
    /// A helper class that calls a delegate on dispose.
    /// </summary>
    public class DisposeHelper : IDisposable
    {
        public delegate void DisposeDelegate();
        private DisposeDelegate _dispose;

        public DisposeHelper(DisposeDelegate dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}
