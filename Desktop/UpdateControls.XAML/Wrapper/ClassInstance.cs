/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstance : CustomTypeDescriptor
    {
        private Type _wrappedType;
        private List<ClassProperty> _classProperties;
        private PropertyDescriptorCollection _propertyDescriptors;

        public ClassInstance(Type wrappedType, Type objectInstanceType)
        {
            _wrappedType = wrappedType;

            // Create a wrapper for each non-collection property.
            _classProperties = _wrappedType
                .GetProperties()
                .Select(p => new ClassProperty(p, objectInstanceType))
                .ToList();
            _propertyDescriptors = new PropertyDescriptorCollection(_classProperties.ToArray());
        }

        public IEnumerable<ClassProperty> ClassProperties
        {
            get { return _classProperties; }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return _propertyDescriptors;
        }

        public override string ToString()
        {
            return _wrappedType.Name;
        }
    }
}
