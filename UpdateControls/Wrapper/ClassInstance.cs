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

namespace UpdateControls.Wrapper
{
    class ClassInstance<ObjectType>
    {
        private List<DependencyPropertyWrapper<ObjectType>> _propertyWrappers;

        public ClassInstance()
        {
            // Create a wrapper for each non-collection property.
            _propertyWrappers = typeof(ObjectType)
                .GetProperties()
                //.Where(p =>
                //    typeof(string).IsAssignableFrom(p.PropertyType) ||
                //    !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))
                .Select(p => new DependencyPropertyWrapper<ObjectType>(p))
                .ToList();
        }

        public IEnumerable<DependencyPropertyWrapper<ObjectType>> PropertyWrappers
        {
            get { return _propertyWrappers; }
        }
    }
}
