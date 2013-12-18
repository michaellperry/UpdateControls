/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstance : DelegatedType
    {
        private Type _wrappedType;
        private Type _objectInstanceType;
        private List<ClassProperty> _classProperties;

        public ClassInstance(Type wrappedType, Type objectInstanceType)
            : base(wrappedType)
        {
            _wrappedType = wrappedType;
            _objectInstanceType = objectInstanceType;

            // Create a wrapper for each non-collection property.
            _classProperties = _wrappedType
                .GetProperties()
                .Select(p => new ClassProperty(p, objectInstanceType))
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

        protected override PropertyInfo DelegatePropertyInfo(PropertyInfo rawPropertyInfo)
        {
            return new ClassProperty(rawPropertyInfo, _objectInstanceType);
        }
    }
}
