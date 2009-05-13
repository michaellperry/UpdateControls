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
    public class ObjectInstance<ObjectType> : DependencyObject, IObjectInstance
    {
        // Wrap the class and all of its property definitions.
        private static ClassInstance<ObjectType> _classWrapper = new ClassInstance<ObjectType>();

        // Wrap the object instance.
        private ObjectType _wrappedObject;

		// Wrap all properties.
		private List<ObjectProperty> _properties;

        public ObjectInstance(ObjectType wrappedObject)
        {
            _wrappedObject = wrappedObject;

            // Create a dependent sentry to monitor each property.
			_properties = _classWrapper.PropertyWrappers.Select(p => ObjectProperty.From(this, p, wrappedObject)).ToList();
        }

        public object WrappedObject
        {
            get { return _wrappedObject; }
        }
    }
}
