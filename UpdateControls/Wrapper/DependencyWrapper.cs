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

namespace UpdateControls.Wrapper
{
    public class DependencyWrapper<ObjectType> : DependencyObject
    {
        // Wrap the class and all of its property definitions.
        private static DependencyClassWrapper<ObjectType> _classWrapper = new DependencyClassWrapper<ObjectType>();

        // Wrap the object instance.
        private ObjectType _wrappedObject;

        public DependencyWrapper(ObjectType wrappedObject)
        {
            _wrappedObject = wrappedObject;

            // Create a dependent sentry to monitor each property.
            foreach (DependencyPropertyWrapper<ObjectType> dependencyPropertyWrapper in _classWrapper.PropertyWrappers)
            {
                if (dependencyPropertyWrapper.CanRead)
                {
                    WrapValueProperty(dependencyPropertyWrapper);
                }
            }
        }

        private void WrapValueProperty(DependencyPropertyWrapper<ObjectType> dependencyPropertyWrapper)
        {
            // When the property is out of date, update it from the wrapped object.
            Dependent depProperty = new Dependent(
                delegate()
                {
                    dependencyPropertyWrapper.UpdateProperty(this, _wrappedObject);
                }
            );

            // When the property becomes out of date, trigger an update.
            Action triggerUpdate = new Action(
                delegate()
                {
                    Dispatcher.BeginInvoke(new Action(
                        delegate()
                        {
                            depProperty.OnGet();
                        }
                    ));
                }
            );
            depProperty.Invalidated += triggerUpdate;

            // The property is out of date right now, so trigger the first update.
            triggerUpdate();
        }

        public ObjectType WrappedObject
        {
            get { return _wrappedObject; }
        }
    }
}
