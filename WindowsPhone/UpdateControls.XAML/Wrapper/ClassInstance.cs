/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    class ClassInstance
    {
        private Type _wrappedType;
        private List<ClassProperty> _classProperties;

        public ClassInstance(Type wrappedType)
        {
            _wrappedType = wrappedType;

            // Create a wrapper for each non-collection property.
            _classProperties = _wrappedType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new ClassProperty(p, _wrappedType))
                .ToList();
        }

        public IEnumerable<ClassProperty> ClassProperties
        {
            get { return _classProperties; }
        }

        public override string ToString()
        {
            return _wrappedType.Name;
        }
    }
}
