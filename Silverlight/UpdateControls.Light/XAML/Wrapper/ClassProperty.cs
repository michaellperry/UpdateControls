/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassProperty
    {
        private static readonly Type[] Primitives = new Type[]
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
			typeof(object),
            typeof(short),
            typeof(ushort),
            typeof(string),
            typeof(ICommand),
            typeof(Type),
            typeof(bool?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(char?),
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
            typeof(int?),
            typeof(uint?),
            typeof(long?),
            typeof(ulong?),
            typeof(short?),
            typeof(ushort?)
        };

        private PropertyInfo _propertyInfo;
        private DependencyProperty _dependencyProperty;
        private Func<IObjectInstance, ObjectProperty> _makeObjectProperty;
        private ConstructorInfo _objectInstanceConstructor;

        public ClassProperty(PropertyInfo property, Type wrappedType)
        {
            _propertyInfo = property;

            // Determine which type of object property to create.
            Type propertyType = property.PropertyType;
            Type valueType;
            if (Primitives.Contains(propertyType))
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
                if (Primitives.Contains(itemType))
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionNative(objectInstance, this);
                else
                {
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollectionObject(objectInstance, this);

                    // Find the type of object instance based on the item type.
                    _objectInstanceConstructor = typeof(ObjectInstance<>)
                        .MakeGenericType(itemType)
                        .GetConstructors()
                        .Single();
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
                    .GetConstructors()
                    .Single();
            }

            // Register a dependency property. XAML can bind to this by name, even though
            // there is no CLR property to be found.
            if (_propertyInfo.CanWrite)
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    valueType,
                    typeof(ObjectInstance<>).MakeGenericType(wrappedType),
                    new PropertyMetadata(OnPropertyChanged));
            }
            else
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    valueType,
                    typeof(ObjectInstance<>).MakeGenericType(wrappedType),
                    new PropertyMetadata(null));
            }
        }

        public ObjectProperty MakeObjectProperty(IObjectInstance objectInstance)
        {
            return _makeObjectProperty(objectInstance);
        }

        public object MakeObjectInstance(object wrappedObject, IDictionary<object, IObjectInstance> wrapperByObject)
        {
            return _objectInstanceConstructor.Invoke(new object[] { wrappedObject, wrapperByObject });
        }

        // Called when the user edits the property. Sets the property in the wrapped object.
		private void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			// Get the wrapped object.
			IObjectInstance objectInstance = (IObjectInstance)obj;
			object wrappedObject = objectInstance.WrappedObject;
			ObjectProperty objectProperty = objectInstance.LookupProperty(this);

            if (objectProperty != null)
            {
                // Set the property in the wrapped object.
                object value = obj.GetValue(_dependencyProperty);
                objectProperty.OnUserInput(value);
            }
        }

        public void SetUserOutput(IObjectInstance objectInstance, object value)
        {
			// Set the value into the dependency property.
            objectInstance.SetValue(_dependencyProperty, value);
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

        public Type PropertyType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", _propertyInfo.DeclaringType.Name, _propertyInfo.Name);
        }
    }
}
