using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace UpdateControls.Wrapper
{
    internal class ObjectPropertyCollectionObject : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionObject(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
			: base(objectInstance, classProperty, wrappedObject)
		{
		}

        public override object TranslateOutgoingValue(object value)
        {
            return new ObjectInstance(value);
        }
    }
}
