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

namespace UpdateControls.Wrapper
{
    class DependencyPropertyWrapper<ObjectType>
    {
        private PropertyInfo _propertyInfo;
        private DependencyProperty _dependencyProperty;

        public DependencyPropertyWrapper(PropertyInfo property)
        {
            _propertyInfo = property;

            // Register a dependency property. XAML can bind to this by name, even though
            // there is no CLR property to be found.
            if (_propertyInfo.CanWrite)
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    _propertyInfo.PropertyType,
                    typeof(DependencyWrapper<ObjectType>),
                    new PropertyMetadata(OnPropertyChanged));
            }
            else
            {
                _dependencyProperty = DependencyProperty.Register(
                    _propertyInfo.Name,
                    _propertyInfo.PropertyType,
                    typeof(DependencyWrapper<ObjectType>),
                    new PropertyMetadata(null));
            }
        }

        // Called when the user edits the property. Sets the property in the wrapped object.
        private void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            // Get the wrapped object.
            ObjectType wrappedObject = ((DependencyWrapper<ObjectType>)obj).WrappedObject;

            // Set the property in the wrapped object.
            _propertyInfo.SetValue(wrappedObject, obj.GetValue(_dependencyProperty), null);
        }

        public void UpdateProperty(DependencyWrapper<ObjectType> obj, ObjectType wrappedObject)
        {
            // Get the property from the wrapped object.
            object value = _propertyInfo.GetValue(wrappedObject, null);
            if (value == null)
                value = DependencyProperty.UnsetValue;
            obj.SetValue(_dependencyProperty, value);
        }

        public bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }
    }
}
