/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System.Collections.Generic;
using System.Linq;
using System;

namespace UpdateControls.Wrapper
{
    class ClassInstance
    {
        private Type _wrappedType;
        private List<ClassProperty> _propertyWrappers;

        public ClassInstance(Type wrappedType)
        {
            _wrappedType = wrappedType;

            // Create a wrapper for each non-collection property.
            _propertyWrappers = _wrappedType
                .GetProperties()
                .Select(p => new ClassProperty(p))
                .ToList();
        }

        public IEnumerable<ClassProperty> PropertyWrappers
        {
            get { return _propertyWrappers; }
        }

        public override string ToString()
        {
            return _wrappedType.Name;
        }
    }
}
