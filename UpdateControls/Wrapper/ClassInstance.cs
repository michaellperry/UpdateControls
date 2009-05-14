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
        private List<ClassProperty> _propertyWrappers;

        public ClassInstance(Type wrappedType)
        {
            // Create a wrapper for each non-collection property.
            _propertyWrappers = wrappedType
                .GetProperties()
                //.Where(p =>
                //    typeof(string).IsAssignableFrom(p.PropertyType) ||
                //    !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))
                .Select(p => new ClassProperty(p))
                .ToList();
        }

        public IEnumerable<ClassProperty> PropertyWrappers
        {
            get { return _propertyWrappers; }
        }
    }
}
