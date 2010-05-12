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
using System.Text.RegularExpressions;

namespace UpdateControls.VSAddIn
{
    public class VBFieldParser
    {
        private static readonly Regex SimpleFieldExpression = new Regex(
            @"^\s*(?<modifier>(private\s+)|(protected\s+)|(public\s+)|())(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s+as\s+(new\s+)?(?<type>[a-zA-Z_][a-zA-Z0-9_]*)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex CollectionFieldExpression = new Regex(
            @"^\s*(?<modifier>(private\s+)|(protected\s+)|(public\s+)|())(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s+as\s+(new\s+)?(?<collection>[a-zA-Z_][a-zA-Z0-9_]*)\s+of\s+(?<type>[a-zA-Z_][a-zA-Z0-9_]*)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private bool _isAtom = false;
        private bool _isCollection = false;
        private string _collection = string.Empty;
        private string _type = string.Empty;
        private string _name = string.Empty;

        public string Line
        {
            set
            {
                _isAtom = false;
                _isCollection = false;
                _collection = string.Empty;
                _type = string.Empty;
                _name = string.Empty;
                Match match = CollectionFieldExpression.Match(value);
                if (match.Success)
                {
                    // Extract the collection, type, and name.
                    _isCollection = true;
                    _collection = match.Groups["collection"].Value;
                    _type = match.Groups["type"].Value;
                    _name = match.Groups["name"].Value;
                }
                else
                {
                    match = SimpleFieldExpression.Match(value);
                    if (match.Success)
                    {
                        // Extract the type and name.
                        _isAtom = true;
                        _type = match.Groups["type"].Value;
                        _name = match.Groups["name"].Value;
                    }
                }
            }
        }

        public bool IsAtom
        {
            get { return _isAtom; }
        }

        public bool IsCollection
        {
            get { return _isCollection; }
        }

        public string Collection
        {
            get { return _collection; }
        }

        public string Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
