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

using System.Reflection;
using System.Windows;
using System;

namespace UpdateControls.Wrapper
{
    class ClassProperty
    {
        private PropertyInfo _propertyInfo;
        private DependencyProperty _dependencyProperty;

        public ClassProperty(PropertyInfo property)
        {
            _propertyInfo = property;

            // Register a dependency property. XAML can bind to this by name, even though
            // there is no CLR property to be found.
            if (_propertyInfo.CanWrite)
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    _propertyInfo.PropertyType,
                    typeof(ObjectInstance),
                    new PropertyMetadata(OnPropertyChanged));
            }
            else
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    _propertyInfo.PropertyType,
                    typeof(ObjectInstance),
                    new PropertyMetadata(null));
            }
        }

        // Called when the user edits the property. Sets the property in the wrapped object.
		private void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			// Get the wrapped object.
			object wrappedObject = ((ObjectInstance)obj).WrappedObject;

			// Set the property in the wrapped object.
			object value = obj.GetValue(_dependencyProperty);
			_propertyInfo.SetValue(wrappedObject, value, null);
		}

        public void UpdateProperty(ObjectInstance objectInstance, object wrappedObject)
        {
            // Get the property from the wrapped object.
            object value = _propertyInfo.GetValue(wrappedObject, null);
            if (value == null)
                value = DependencyProperty.UnsetValue;
            objectInstance.SetValue(_dependencyProperty, value);
        }

        public bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }

        public Type PropertyType
        {
            get { return _propertyInfo.PropertyType; }
        }
    }
}
