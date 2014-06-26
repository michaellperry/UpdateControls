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
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstance : CustomTypeDescriptor
    {
        private Type _wrappedType;
        private List<ClassMember> _classProperties;
        private PropertyDescriptorCollection _propertyDescriptors;
        private List<ClassEvent> _classEvents;
        private EventDescriptorCollection _eventDescriptors;

        public ClassInstance(Type wrappedType, Type objectInstanceType)
        {
            _wrappedType = wrappedType;

            // Create a wrapper for each non-collection property.
            _classProperties = _wrappedType
                .GetProperties()
                .Select(p => ClassMemberIndependent.Intercept(new ClassMemberProperty(p, objectInstanceType)))
                .Concat<ClassMember>(_wrappedType
                    .GetFields()
                    .Select(f => ClassMemberIndependent.Intercept(new ClassMemberField(f, objectInstanceType))))
                .ToList();
            _classProperties.AddRange(
                (from method in _wrappedType.GetMethods()
                 where method.ReturnType == typeof(void) && method.GetParameters().Length == 0
                 let can = (from p in _classProperties
                            where p.Name == "Can" + method.Name && p.PropertyType == typeof(bool)
                            select p).FirstOrDefault()
                 select new ClassMemberCommand(method, can, objectInstanceType)).ToList());
            _propertyDescriptors = new PropertyDescriptorCollection(_classProperties.ToArray());

            // Create a pass-through for each event.
            _classEvents = _wrappedType
                .GetEvents()
                .Select(e => new ClassEvent(e))
                .ToList();
            _eventDescriptors = new EventDescriptorCollection(_classEvents.ToArray());
        }

        public IEnumerable<ClassMember> ClassProperties
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
