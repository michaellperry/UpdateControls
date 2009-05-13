using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;

namespace UpdateControls.Wrapper
{
    abstract class ObjectProperty
	{
		protected DependencyObject _dependencyObject;
		protected IClassProperty _classProperty;
		protected object _wrappedObject;

		public abstract object TranslateIncommingValue(object value);
		public abstract object TranslateOutgoingValue(object value);

		public ObjectProperty(DependencyObject dependencyObject, IClassProperty classProperty, object wrappedObject)
		{
			_dependencyObject = dependencyObject;
			_classProperty = classProperty;
			_wrappedObject = wrappedObject;
		}

		public static ObjectProperty From(DependencyObject dependencyObject, IClassProperty classProperty, object wrappedObject)
		{
			// TODO: Determine which type of object property to create.
			ObjectProperty objectProperty = new ObjectPropertyNative(dependencyObject, classProperty, wrappedObject);

			return objectProperty;
		}
	}
}
