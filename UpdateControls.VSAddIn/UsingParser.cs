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
using System.Text.RegularExpressions;

namespace UpdateControls.VSAddIn
{
    public class UsingParser
    {
        private Regex _usingUpdateControls = new Regex(@"using\s+UpdateControls\s*;");
        private Regex _using = new Regex(@"using");
        private Regex _namespace = new Regex(@"namespace");

        private bool _isUsingUpdateControls = false;
        private bool _isUsing = false;
        private bool _isNamespace = false;

        public string Line
        {
            set
            {
                _isUsingUpdateControls = false;
                _isUsing = false;
                _isNamespace = false;

                Match match = _usingUpdateControls.Match(value);
                if (match.Success)
                {
                    _isUsingUpdateControls = true;
                    return;
                }
                match = _using.Match(value);
                if (match.Success)
                {
                    _isUsing = true;
                    return;
                }
                match = _namespace.Match(value);
                if (match.Success)
                {
                    _isNamespace = true;
                    return;
                }
            }
        }


        public bool IsUsingUpdateControls
        {
            get { return _isUsingUpdateControls; }
        }

        public bool IsUsing
        {
            get { return _isUsing; }
        }

        public bool IsNamespace
        {
            get { return _isNamespace; }
        }
    }
}
