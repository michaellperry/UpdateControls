using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace UpdateControls.Wrapper
{
	internal abstract class ObjectPropertyCollection : ObjectProperty
	{
		private ObservableCollection<object> _collection = new ObservableCollection<object>();

        public ObjectPropertyCollection(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
			: base(objectInstance, classProperty, wrappedObject)
		{
		}

		public override void OnUserInput(object value)
		{
			throw new NotSupportedException("Update Controls does not support two-way binding of collection properties.");
		}

        public abstract object TranslateOutgoingValue(object value);
    }
}
