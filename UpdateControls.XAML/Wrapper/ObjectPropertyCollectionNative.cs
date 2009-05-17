using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;

namespace UpdateControls.Wrapper
{
	internal class ObjectPropertyCollectionNative : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionNative(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
			: base(objectInstance, classProperty, wrappedObject)
		{
		}

        public override object TranslateOutgoingValue(object value)
        {
            return value;
        }
    }
}
