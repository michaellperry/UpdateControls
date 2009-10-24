/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassProperty : PropertyDescriptor
    {
        private static readonly Type[] Primitives = new Type[]
        {
			typeof(object),
            typeof(string),
            typeof(ICommand)
        };

        private static readonly Type[] Bindables = new Type[]
        {
            typeof(DependencyObject),
            typeof(INotifyPropertyChanged),
            typeof(INotifyCollectionChanged)
        };

        private PropertyInfo _propertyInfo;
        private Type _objectInstanceType;
		private Type _valueType;
        private Func<IObjectInstance, ObjectProperty> _makeObjectProperty;
        private ConstructorInfo _objectInstanceConstructor;

        public ClassProperty(PropertyInfo property, Type objectInstanceType)
            : base(property.Name, null)
        {
            _propertyInfo = property;
            _objectInstanceType = objectInstanceType;

            // Determine which type of object property to create.
            Type propertyType = property.PropertyType;
            Type valueType;
			if (IsPrimitive(propertyType))
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomNative(objectInstance, this);
                valueType = propertyType;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                // Figure out what it's an IEnumerable of.
                Type itemType;
                if (propertyType.GetGenericArguments().Length == 1)
                    itemType = propertyType.GetGenericArguments()[0];
                else
                    itemType = typeof(object);
                if (IsPrimitive(itemType))
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionNative(objectInstance, this);
                else
                {
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionObject(objectInstance, this);

                    // Find the type of object instance based on the item type.
                    _objectInstanceConstructor = typeof(ObjectInstance<>)
                        .MakeGenericType(itemType)
                        .GetConstructor(new Type[] { typeof(object), typeof(Dispatcher) });
                }
                valueType = typeof(IEnumerable);
            }
            else
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomObject(objectInstance, this);
                valueType = typeof(ObjectInstance<>).MakeGenericType(propertyType);

                // Find the type of object instance based on the property type.
                _objectInstanceConstructor = valueType
                    .GetConstructor(new Type[] { typeof(object), typeof(Dispatcher) });
            }

			_valueType = valueType;
        }

        public ObjectProperty MakeObjectProperty(IObjectInstance objectInstance)
        {
            return _makeObjectProperty(objectInstance);
        }

        public object MakeObjectInstance(object wrappedObject, Dispatcher dispatcher)
        {
            return _objectInstanceConstructor.Invoke(new object[] { wrappedObject, dispatcher });
        }

		public object GetObjectValue(object wrappedObject)
		{
			// Get the property from the wrapped object.
			return _propertyInfo.GetValue(wrappedObject, null);
		}

		public void SetObjectValue(object wrappedObject, object value)
		{
			_propertyInfo.SetValue(wrappedObject, value, null);
		}

		public bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }

        public override Type PropertyType
        {
            get { return _valueType; }
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", _propertyInfo.DeclaringType.Name, _propertyInfo.Name);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return _objectInstanceType; }
        }

        public override object GetValue(object component)
        {
            return GetObjectProperty(component).Value;
        }

        public override bool IsReadOnly
        {
            get { return !_propertyInfo.CanWrite; }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            GetObjectProperty(component).OnUserInput(value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        private ObjectProperty GetObjectProperty(object component)
        {
            // Find the object property.
            IObjectInstance objectInstance = ((IObjectInstance)component);
            ObjectProperty objectProperty = objectInstance.LookupProperty(this);
            return objectProperty;
        }

		private static bool IsPrimitive(Type type)
		{
            return
                type.IsValueType ||
                type.IsPrimitive ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) ||
                Primitives.Contains(type) ||
                // Don't wrap objects that are already bindable
                Bindables.Any(b => b.IsAssignableFrom(type));
		}
	}
}
