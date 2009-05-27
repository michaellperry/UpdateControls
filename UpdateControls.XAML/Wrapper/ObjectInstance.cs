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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    [TypeDescriptionProvider(typeof(ClassInstanceProvider))]
    public class ObjectInstance<TWrappedObjectType> : IObjectInstance, INotifyPropertyChanged
    {
        // Wrap the class and all of its property definitions.
		private static ClassInstance _classInstance = new ClassInstance(typeof(TWrappedObjectType), typeof(ObjectInstance<TWrappedObjectType>));

        // Wrap the object instance.
        private object _wrappedObject;

		// Wrap all properties.
		private List<ObjectProperty> _properties;

		public ObjectInstance(object wrappedObject)
		{
			_wrappedObject = wrappedObject;

            // Create a wrapper around each property.
            _properties = _classInstance.ClassProperties.Select(p => ObjectProperty.From(this, p)).ToList();
		}

        public ClassInstance ClassInstance
        {
            get { return _classInstance; }
        }

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }

		public ObjectProperty LookupProperty(ClassProperty classProperty)
		{
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

        public void FirePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
