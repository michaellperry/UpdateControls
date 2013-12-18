/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
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
using System.Windows.Markup;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassProperty : DelegatedPropertyInfo
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
        private Type _objectInstanceType;
        private Func<IObjectInstance, ObjectProperty> _makeObjectProperty;

        public ClassProperty(PropertyInfo property, Type objectInstanceType)
            : base(property, GetValueType(property.PropertyType))
        {
            _propertyInfo = property;
            _objectInstanceType = objectInstanceType;

            // Determine which type of object property to create.
            Type propertyType = property.PropertyType;
			if (IsPrimitive(propertyType))
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomNative(objectInstance, this);
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
                }
            }
            else
            {
                _makeObjectProperty = objectInstance =>
                    new ObjectPropertyAtomObject(objectInstance, this);
            }
        }

        private static Type GetValueType(Type propertyType)
        {
            if (IsPrimitive(propertyType))
                return propertyType;
            else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                return typeof(IEnumerable);
            else
                return typeof(IObjectInstance);
        }

        public ObjectProperty MakeObjectProperty(IObjectInstance objectInstance)
        {
            return _makeObjectProperty(objectInstance);
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

        protected override object GetValue(object obj)
        {
            IObjectInstance objectInsance = obj as IObjectInstance;
            ObjectProperty property = objectInsance.LookupProperty(this);
            return property.Value;
        }

        protected override void SetValue(object obj, object value)
        {
            IObjectInstance objectInsance = obj as IObjectInstance;
            ObjectProperty property = objectInsance.LookupProperty(this);
            property.OnUserInput(value);
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
