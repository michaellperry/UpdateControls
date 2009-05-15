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
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Wrapper
{
    public class ObjectInstance : DependencyObject
    {
        // Wrap the class and all of its property definitions.
		private static Dictionary<Type, ClassInstance> _classInstanceByType = new Dictionary<Type, ClassInstance>();

        // Wrap the object instance.
        private object _wrappedObject;

		// Wrap all properties.
		private List<ObjectProperty> _properties;

		public ObjectInstance(object wrappedObject)
		{
			_wrappedObject = wrappedObject;

			// Ensure that we have a class wrapper for this type.
			ClassInstance classInstance;
			Type wrappedType = wrappedObject.GetType();
			if (!_classInstanceByType.TryGetValue(wrappedType, out classInstance))
			{
				classInstance = new ClassInstance(wrappedType);
				_classInstanceByType.Add(wrappedType, classInstance);
			}

			// Create a dependent sentry to monitor each property.
			_properties = classInstance.PropertyWrappers.Select(p => ObjectProperty.From(this, p, wrappedObject)).ToList();
		}

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }

		internal ObjectProperty LookupProperty(ClassProperty classProperty)
		{
			return _properties.Single(p => p.ClassProperty == classProperty);
		}
	}
}
