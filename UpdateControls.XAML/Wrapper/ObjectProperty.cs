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
        public abstract object Value { get; }

        public static ObjectProperty From(IObjectInstance objectInstance, ClassProperty classProperty)
		{
			return classProperty.MakeObjectProperty(objectInstance);
		}

        public override string ToString()
        {
            return String.Format("{0}({1})", ClassProperty, ObjectInstance.WrappedObject);
        }
	}
}
