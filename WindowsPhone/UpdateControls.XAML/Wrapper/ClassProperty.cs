/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
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
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Markup;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassProperty
    {
        private static readonly Type[] Primitives = new Type[]
        {
			typeof(object),
            typeof(string),
            typeof(Uri),
            typeof(Cursor)
        };

        private static readonly Type[] Bindables = new Type[]
        {
            typeof(DependencyObject),
            typeof(INotifyPropertyChanged),
            typeof(INotifyCollectionChanged),
            typeof(ICommand),
            typeof(InputScope),
            typeof(XmlLanguage)
        };

        private PropertyInfo _propertyInfo;
        private DependencyProperty _dependencyProperty;
        private Func<IObjectInstance, ObjectProperty> _makeObjectProperty;

        public ClassProperty(PropertyInfo property, Type wrappedType)
        {
            _propertyInfo = property;

            // Determine which type of object property to create.
            Type propertyType = property.PropertyType;
            Type valueType;
            if (IsPrimitive(propertyType))
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtom(objectInstance, this, false);
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
                        new ObjectPropertyCollection(objectInstance, this, false);
                else
                {
                    _makeObjectProperty = objectInstance =>
                        new ObjectPropertyCollection(objectInstance, this, true);
                }
                valueType = typeof(IEnumerable);
            }
            else
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtom(objectInstance, this, true);
                valueType = typeof(IObjectInstance);
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

		public DependencyProperty DependencyProperty
		{
			get { return _dependencyProperty; }
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
            if (_propertyInfo.CanWrite)
    			_propertyInfo.SetValue(wrappedObject, value, null);
		}

		public string Name
		{
			get { return _propertyInfo.Name; }
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
