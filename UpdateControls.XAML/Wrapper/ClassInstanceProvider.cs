using System;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassInstanceProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return ((IObjectInstance)instance).ClassInstance;
        }
    }
}
