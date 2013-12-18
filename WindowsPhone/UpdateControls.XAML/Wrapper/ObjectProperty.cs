/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using System.Linq;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class ObjectProperty : IDisposable
	{
		public IObjectInstance ObjectInstance { get; private set; }
		public ClassProperty ClassProperty { get; private set; }

        public ObjectProperty(IObjectInstance objectInstance, ClassProperty classProperty)
		{
			ObjectInstance = objectInstance;
			ClassProperty = classProperty;
		}

		public abstract void OnUserInput(object value);
        public abstract void UpdateNodes();
        public abstract void Dispose();

        public static ObjectProperty From(IObjectInstance objectInstance, ClassProperty classProperty)
		{
			return classProperty.MakeObjectProperty(objectInstance);
		}

        public override string ToString()
        {
            return String.Format("{0}({1})", ClassProperty, ObjectInstance.WrappedObject);
        }

        protected bool WrapObject(object value, out IObjectInstance wrapper)
        {
            return ObjectInstance.Tree.WrapObject(value, out wrapper);
        }
	}
}
