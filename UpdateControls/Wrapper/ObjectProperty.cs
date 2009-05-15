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
            typeof(double),
			typeof(object)
        };

        protected ObjectInstance _objectInstance;
		protected ClassProperty _classProperty;
		protected object _wrappedObject;

        public ObjectProperty(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
		{
			_objectInstance = objectInstance;
			_classProperty = classProperty;
			_wrappedObject = wrappedObject;
		}

        public static ObjectProperty From(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
		{
			// Determine which type of object property to create.
			ObjectProperty objectProperty;
			Type propertyType = classProperty.PropertyType;
			if (Primitives.Contains(propertyType))
			{
				objectProperty = new ObjectPropertyAtomNative(objectInstance, classProperty, wrappedObject);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				// Figure out what it's an IEnumerable of.
				Type itemType;
				if (propertyType.GetGenericArguments().Length == 1)
					itemType = propertyType.GetGenericArguments()[0];
				else
					itemType = typeof(object);
				if (Primitives.Contains(itemType))
					objectProperty = new ObjectPropertyCollectionNative(objectInstance, classProperty, wrappedObject);
				else
					objectProperty = new ObjectPropertyCollectionObject(objectInstance, classProperty, wrappedObject);
			}
			else
			{
				objectProperty = new ObjectPropertyAtomObject(objectInstance, classProperty, wrappedObject);
			}

			return objectProperty;
		}
	}
}
