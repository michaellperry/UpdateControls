using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
    class ObjectPropertyAtomObject : ObjectPropertyAtom
    {
        public ObjectPropertyAtomObject(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
            : base(objectInstance, classProperty, wrappedObject)
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
