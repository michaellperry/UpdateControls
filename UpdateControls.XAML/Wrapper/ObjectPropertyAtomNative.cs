using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
	class ObjectPropertyAtomNative : ObjectPropertyAtom
    {

        public ObjectPropertyAtomNative(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
			: base(objectInstance, classProperty, wrappedObject)
		{
        }
        public override object TranslateIncommingValue(object value)
		{
			return value;
		}

		public override object TranslateOutgoingValue(object value)
		{
			return value;
		}
	}
}
