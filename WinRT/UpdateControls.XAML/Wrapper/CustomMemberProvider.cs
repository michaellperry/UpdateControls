using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace UpdateControls.XAML.Wrapper
{
    class CustomMemberProvider : IXamlMember
    {
        private static readonly TypeInfo[] Primitives = new TypeInfo[]
        {
			typeof(object).GetTypeInfo(),
            typeof(string).GetTypeInfo(),
            typeof(ICommand).GetTypeInfo()
        };

        private static readonly TypeInfo[] Bindables = new TypeInfo[]
        {
            typeof(DependencyObject).GetTypeInfo(),
            typeof(INotifyPropertyChanged).GetTypeInfo(),
            typeof(INotifyCollectionChanged).GetTypeInfo()
        };

        private static TypeInfo EnumerableTypeInfo = typeof(IEnumerable).GetTypeInfo();

        private readonly CustomTypeProvider _owner;
        private readonly Type _type;
        private readonly string _name;
        private readonly PropertyInfo _propertyInfo;
        private readonly bool _isPrimitive;
        private readonly bool _isCollection;

        public CustomMemberProvider(CustomTypeProvider owner, Type type, string name)
        {
            _owner = owner;
            _name = name;
            _type = type;

            _propertyInfo = _type.GetRuntimeProperty(_name);
            var propertyTypeInfo = _propertyInfo.PropertyType.GetTypeInfo();
            if (TypeIsPrimitive(propertyTypeInfo))
            {
                _isCollection = false;
                _isPrimitive = true;
            }
            else if (IsCollectionType(propertyTypeInfo))
            {
                _isCollection = true;
                _isPrimitive = TypeIsPrimitive(GetItemType(propertyTypeInfo));
            }
            else
            {
                _isCollection = false;
                _isPrimitive = false;
            }
        }

        public bool IsCollection
        {
            get { return _isCollection; }
        }

        public bool IsPrimitive
        {
            get { return _isPrimitive; }
        }

        public object GetValue(object instance)
        {
            IDependentObject obj = instance as IDependentObject;
            if (obj == null)
                return null;

            DependentProperty property = obj.GetDependentProperty(this);
            if (property == null)
                return null;

            return property.GetValue();
        }

        public bool IsAttachable
        {
            get { return false; }
        }

        public bool IsDependencyProperty
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return !_propertyInfo.CanWrite; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void SetValue(object instance, object value)
        {
            throw new NotImplementedException();
        }

        public IXamlType TargetType
        {
            get { throw new NotImplementedException(); }
        }

        public IXamlType Type
        {
            get { return _owner; }
        }

        private static bool IsCollectionType(TypeInfo propertyType)
        {
            return EnumerableTypeInfo.IsAssignableFrom(propertyType);
        }

        private static TypeInfo GetItemType(TypeInfo collectionType)
        {
            if (collectionType.GenericTypeArguments.Length == 1)
                return collectionType.GenericTypeArguments[0].GetTypeInfo();
            else
                return typeof(object).GetTypeInfo();
        }

        private static bool TypeIsPrimitive(TypeInfo typeInfo)
        {
            return
                typeInfo.IsValueType ||
                typeInfo.IsPrimitive ||
                (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)) ||
                Primitives.Contains(typeInfo) ||
                // Don't wrap objects that are already bindable
                Bindables.Any(b => b.IsAssignableFrom(typeInfo));
        }
    }
}
