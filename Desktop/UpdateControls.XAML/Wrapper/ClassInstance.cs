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
        private List<ClassEvent> _classEvents;
        private EventDescriptorCollection _eventDescriptors;

        public ClassInstance(Type wrappedType, Type objectInstanceType)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("new ClassInstance({0}, {1})", wrappedType, objectInstanceType));
            _wrappedType = wrappedType;

            // Create a wrapper for each non-collection property.
            _classProperties = _wrappedType
                .GetProperties()
                .Select(p => new ClassProperty(p, objectInstanceType))
                .ToList();
            _propertyDescriptors = new PropertyDescriptorCollection(_classProperties.ToArray());

            // Create a pass-through for each event.
            _classEvents = _wrappedType
                .GetEvents()
                .Select(e => new ClassEvent(e))
                .ToList();
            _eventDescriptors = new EventDescriptorCollection(_classEvents.ToArray());
        }

        public IEnumerable<ClassProperty> ClassProperties
        {
            get { return _classProperties; }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return _propertyDescriptors;
        }

        public override EventDescriptorCollection GetEvents()
        {
            return _eventDescriptors;
        }

        public override EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return _eventDescriptors;
        }

        public override string ToString()
        {
            return _wrappedType.Name;
        }
    }
}
