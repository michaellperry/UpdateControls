using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
    class ObjectPropertyObject : ObjectProperty
    {
        public ObjectPropertyObject(DependencyObject dependencyObject, ClassProperty classProperty, object wrappedObject)
            : base(dependencyObject, classProperty, wrappedObject)
        {
        }

        public override object TranslateIncommingValue(object value)
        {
            return ((ObjectInstance)value).WrappedObject;
        }

        public override object TranslateOutgoingValue(object value)
        {
            return new ObjectInstance(value);
        }
    }
}
