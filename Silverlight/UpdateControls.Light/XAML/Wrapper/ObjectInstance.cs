/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UpdateControls.XAML.Wrapper
{
    public class ObjectInstance<TWrappedObjectType> : DependencyObject, IObjectInstance
    {
        // Wrap the class and all of its property definitions.
		private static ClassInstance _classInstance = new ClassInstance(typeof(TWrappedObjectType));

        // Wrap the object instance.
        private object _wrappedObject;

        // The collection of wrapped objects.
        private IDictionary<object, IObjectInstance> _wrapperByObject;

		// Wrap all properties.
		private List<ObjectProperty> _properties;

        public ObjectInstance(TWrappedObjectType wrappedObject, IDictionary<object, IObjectInstance> wrapperByObject)
		{
			_wrappedObject = wrappedObject;
            _wrapperByObject = wrapperByObject;

            // Save this wrapper in the collection.
            _wrapperByObject.Add(wrappedObject, this);

            // Create a wrapper around each property.
            _properties = _classInstance.ClassProperties.Select(p => ObjectProperty.From(this, p)).ToList();
		}

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }

        public IDictionary<object, IObjectInstance> WrapperByObject
        {
            get { return _wrapperByObject; }
        }

        public ObjectProperty LookupProperty(ClassProperty classProperty)
        {
            if (_properties == null)
                return null;
            else
    			return _properties.Single(p => p.ClassProperty == classProperty);
		}

        public override string ToString()
        {
            return _wrappedObject.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            ObjectInstance<TWrappedObjectType> that = (ObjectInstance<TWrappedObjectType>)obj;
            return Object.Equals(this._wrappedObject, that._wrappedObject);
        }

        public override int GetHashCode()
        {
            return _wrappedObject.GetHashCode();
        }
	}
}
