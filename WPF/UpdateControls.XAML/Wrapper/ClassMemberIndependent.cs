using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.Fields;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassMemberIndependent : ClassMember
    {
        ClassMember _independent;
        PropertyInfo _valueProperty;

        public ClassMemberIndependent(ClassMember independent)
            : base(independent.Name, UnwrapType(independent.UnderlyingType), independent.ComponentType)
        {
            _independent = independent;
            _valueProperty = independent.UnderlyingType.GetProperty("Value");
        }

		public override object GetObjectValue(object wrappedObject)
		{
            return _valueProperty.GetValue(_independent.GetObjectValue(wrappedObject));
		}

        public override void SetObjectValue(object wrappedObject, object value)
		{
            _valueProperty.SetValue(_independent.GetObjectValue(wrappedObject), value);
		}

		public override bool CanRead
        {
            get { return true; }
        }

		public override Type UnderlyingType
		{
            get { return UnwrapType(_independent.UnderlyingType); }
		}

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public static ClassMember Intercept(ClassMember member)
        {
            if (IsIndependent(member.UnderlyingType))
                return new ClassMemberIndependent(member);
            else
                return member;
        }

        public static Type UnwrapType(Type independentType)
        {
            for (Type ancestor = independentType; ancestor != typeof(object) && ancestor != null; ancestor = ancestor.BaseType)
                if (ancestor.IsGenericType && ancestor.GetGenericTypeDefinition() == typeof(Independent<>))
                    return ancestor.GenericTypeArguments[0];
            return null;
        }

        public static bool IsIndependent(Type memberType)
        {
            return UnwrapType(memberType) != null;
        }
    }
}
