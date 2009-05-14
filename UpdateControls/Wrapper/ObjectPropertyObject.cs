using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
    class ObjectPropertyObject : ObjectProperty
    {
        public ObjectPropertyObject(DependencyObject dependencyObject, IClassProperty classProperty, object wrappedObject)
            : base(dependencyObject, classProperty, wrappedObject)
        {
        }

        public override object TranslateIncommingValue(object value)
        {
            return ((IObjectInstance)value).WrappedObject;
        }

        public override object TranslateOutgoingValue(object value)
        {
            // TODO: Create the right kind of wrapper!
            return new ObjectInstance<object>(value);
        }
    }
}
