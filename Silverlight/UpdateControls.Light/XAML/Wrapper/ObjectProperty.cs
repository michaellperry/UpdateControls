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
using System.Linq;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class ObjectProperty
	{
		public IObjectInstance ObjectInstance { get; private set; }
		public ClassProperty ClassProperty { get; private set; }

        public ObjectProperty(IObjectInstance objectInstance, ClassProperty classProperty)
		{
			ObjectInstance = objectInstance;
			ClassProperty = classProperty;
		}

		public abstract void OnUserInput(object value);

        public static ObjectProperty From(IObjectInstance objectInstance, ClassProperty classProperty)
		{
			return classProperty.MakeObjectProperty(objectInstance);
		}

        public override string ToString()
        {
            return String.Format("{0}({1})", ClassProperty, ObjectInstance.WrappedObject);
        }

        protected object WrapObject(object value)
        {
            // Try to reuse the wrapper we've already created.
            IObjectInstance wrapper;
            if (ObjectInstance.WrapperByObject.TryGetValue(value, out wrapper))
                return wrapper;
            else
            {
                return typeof(ObjectInstance<>)
                    .MakeGenericType(value.GetType())
                    .GetConstructors()
                    .Single()
                    .Invoke(new object[] { value, ObjectInstance.WrapperByObject });
            }
        }
	}
}
