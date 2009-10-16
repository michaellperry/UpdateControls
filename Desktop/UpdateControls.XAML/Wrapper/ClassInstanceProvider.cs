using System;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstanceProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            IObjectInstance objectInstance = instance as IObjectInstance;
            if (objectInstance != null)
                return objectInstance.ClassInstance;

            return new ClassInstance(objectType, typeof(ObjectInstance<>).MakeGenericType(objectType));
        }
    }
}
