using System;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    public abstract class DelegatedType : Type
    {
        private Type _rawType;
        private Dictionary<string, PropertyInfo> _propertyByName = new Dictionary<string,PropertyInfo>();

        public DelegatedType(Type rawType)
        {
            _rawType = rawType;
        }

        protected abstract PropertyInfo DelegatePropertyInfo(PropertyInfo rawPropertyInfo);

        public override Assembly Assembly
        {
            get { return _rawType.Assembly; }
        }

        public override string AssemblyQualifiedName
        {
            get { return _rawType.AssemblyQualifiedName; }
        }

        public override Type BaseType
        {
            get { return _rawType.BaseType; }
        }

        public override string FullName
        {
            get { return _rawType.FullName; }
        }

        public override Guid GUID
        {
            get { return _rawType.GUID; }
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return new TypeAttributes();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _rawType.GetConstructor(bindingAttr, binder, types, modifiers);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return _rawType.GetConstructors(bindingAttr);
        }

        public override Type GetElementType()
        {
            return _rawType.GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _rawType.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _rawType.GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _rawType.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _rawType.GetFields(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _rawType.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return _rawType.GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return _rawType.GetMembers(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _rawType.GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return _rawType.GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return _rawType.GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return _rawType.GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return _rawType.GetProperties(bindingAttr);
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            PropertyInfo result;
            if (!_propertyByName.TryGetValue(name, out result))
            {
                PropertyInfo propertyInfo = _rawType.GetProperty(name, bindingAttr);
                if (propertyInfo == null)
                    return null;
                result = DelegatePropertyInfo(propertyInfo);
                _propertyByName.Add(name, result);
            }
            return result;
        }

        protected override bool HasElementTypeImpl()
        {
            return _rawType.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return _rawType.InvokeMember(name, invokeAttr, binder, target, args);
        }

        protected override bool IsArrayImpl()
        {
            return _rawType.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return _rawType.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return _rawType.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return _rawType.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return _rawType.IsPrimitive;
        }

        public override Module Module
        {
            get { return _rawType.Module; }
        }

        public override string Namespace
        {
            get { return _rawType.Namespace; }
        }

        public override Type UnderlyingSystemType
        {
            get { return _rawType.UnderlyingSystemType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _rawType.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _rawType.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _rawType.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _rawType.Name; }
        }
    }
}
