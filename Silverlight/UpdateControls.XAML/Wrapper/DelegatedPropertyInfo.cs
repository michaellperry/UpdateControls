using System;
using System.Globalization;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class DelegatedPropertyInfo : PropertyInfo
    {
        private readonly PropertyInfo _rawPropertyInfo;
        private readonly Type _propertyType;

        public DelegatedPropertyInfo(PropertyInfo rawPropertyInfo, Type propertyType)
        {
            _rawPropertyInfo = rawPropertyInfo;
            _propertyType = propertyType;
        }

        protected abstract object GetValue(object obj);
        protected abstract void SetValue(object obj, object value);

        public override PropertyAttributes Attributes
        {
            get { return _rawPropertyInfo.Attributes; }
        }

        public override bool CanRead
        {
            get { return _rawPropertyInfo.CanRead; }
        }

        public override bool CanWrite
        {
            get { return _rawPropertyInfo.CanWrite; }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return _rawPropertyInfo.GetAccessors(nonPublic);
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return _rawPropertyInfo.GetGetMethod(nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return _rawPropertyInfo.GetIndexParameters();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _rawPropertyInfo.GetSetMethod(nonPublic);
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            return GetValue(obj);
        }

        public override Type PropertyType
        {
            get { return _propertyType; }
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            SetValue(obj, value);
        }

        public override Type DeclaringType
        {
            get { return _rawPropertyInfo.DeclaringType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _rawPropertyInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _rawPropertyInfo.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _rawPropertyInfo.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _rawPropertyInfo.Name; }
        }

        public override Type ReflectedType
        {
            get { return _rawPropertyInfo.ReflectedType; }
        }
    }
}
