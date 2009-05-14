using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;

namespace UpdateControls.Wrapper
{
    abstract class ObjectProperty
	{
        private static readonly Type[] Primitives = new Type[]
        {
            typeof(string),
            typeof(int),
            typeof(decimal),
            typeof(float),
            typeof(short),
            typeof(long),
            typeof(double)
        };

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
			// Determine which type of object property to create.
            ObjectProperty objectProperty;
            if (Primitives.Contains(classProperty.PropertyType))
            {
                objectProperty = new ObjectPropertyNative(dependencyObject, classProperty, wrappedObject);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(classProperty.PropertyType))
            {
                // TODO: Figure out what it's an IEnumerable of.
                objectProperty = null;
            }
            else
            {
                objectProperty = new ObjectPropertyObject(dependencyObject, classProperty, wrappedObject);
            }

			return objectProperty;
		}
	}
}
