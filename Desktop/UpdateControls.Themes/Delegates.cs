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
using System.Windows.Forms;

namespace UpdateControls.Themes
{
    /// <summary>
    /// A delegate type used by update controls.
    /// </summary>
    public delegate void SetObjectIntegerDelegate(object tag, int value);
    /// <summary>
    /// A delegate type used by update controls.
    /// </summary>
    public delegate float GetFloatDelegate();
    /// <summary>
    /// A delegate type used by update controls.
    /// </summary>
    public delegate Control CreateControlDelegate(object tag);
    /// <summary>
    /// A delegate type used by update controls.
    /// </summary>
    public delegate System.Drawing.Image GetObjectImageDelegate(object tag);
}
