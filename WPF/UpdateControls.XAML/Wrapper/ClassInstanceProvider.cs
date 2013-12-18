using System;
using System.ComponentModel;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstanceProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            IObjectInstance objectInstance = instance as IObjectInstance;
            if (objectInstance != null)
                return objectInstance.ClassInstance;

            if (objectType.IsGenericType)
            {
                var genericRoot = objectType.GetGenericTypeDefinition();
                if (genericRoot == typeof(ObjectInstance<>))
                {
                    var field = objectType.GetField("_classInstance",
                        BindingFlags.NonPublic |
                        BindingFlags.Static |
                        BindingFlags.DeclaredOnly);
                    var value = field.GetValue(null);
                    return value as ICustomTypeDescriptor;
                }
            }

            return new ClassInstance(objectType, typeof(ObjectInstance<>).MakeGenericType(objectType));
        }
    }
}
