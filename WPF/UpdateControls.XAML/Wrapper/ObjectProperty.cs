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

        public void OnUserInput(object value) { BindingInterceptor.Current.SetValue(this, value); }
        public object Value { get { return BindingInterceptor.Current.GetValue(this); } }

        internal object ContinueGetValue() { return GetValue(); }
        internal void ContinueSetValue(object value) { SetValue(value); }
        internal void ContinueUpdateValue() { UpdateValue(); }

        protected abstract object GetValue();
        protected abstract void SetValue(object value);
        protected abstract void UpdateValue();

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
