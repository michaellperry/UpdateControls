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
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class ObjectProperty
	{
		public IObjectInstance ObjectInstance { get; private set; }
		public ClassMember ClassProperty { get; private set; }

        public ObjectProperty(IObjectInstance objectInstance, ClassMember classProperty)
		{
			ObjectInstance = objectInstance;
			ClassProperty = classProperty;
		}

		public abstract void OnUserInput(object value);
        public abstract object Value { get; }

        public static ObjectProperty From(IObjectInstance objectInstance, ClassMember classProperty)
        {
            return classProperty.MakeObjectProperty(objectInstance);
        }

        public override string ToString()
        {
            return String.Format("{0}({1})", ClassProperty, ObjectInstance.WrappedObject);
        }

        protected object WrapObject(object value)
        {
            return typeof(ObjectInstance<>)
				.MakeGenericType(value.GetType())
				.GetConstructor(new Type[] { typeof(object), typeof(Dispatcher) })
				.Invoke(new object[] { value, ObjectInstance.Dispatcher });
		}
	}
}
