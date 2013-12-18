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
            typeof(string).GetTypeInfo()
        };

        private static readonly TypeInfo[] Bindables = new TypeInfo[]
        {
            typeof(DependencyObject).GetTypeInfo(),
            typeof(INotifyPropertyChanged).GetTypeInfo(),
            typeof(INotifyCollectionChanged).GetTypeInfo(),
            typeof(ICommand).GetTypeInfo()
        };

        private static TypeInfo EnumerableTypeInfo = typeof(IEnumerable).GetTypeInfo();

        private readonly CustomTypeProvider _owner;
        private readonly Type _type;
        private readonly string _name;
        private readonly PropertyInfo _propertyInfo;
        private readonly bool _isPrimitive;
        private readonly bool _isCollection;

        private CustomMemberProvider(CustomTypeProvider owner, Type type, string name, PropertyInfo propertyInfo, bool isPrimitive, bool isCollection)
        {
            _owner = owner;
            _type = type;
            _name = name;
            _propertyInfo = propertyInfo;
            _isPrimitive = isPrimitive;
            _isCollection = isCollection;
        }

        public static CustomMemberProvider For(CustomTypeProvider owner, Type type, string name)
        {
            var propertyInfo = type.GetRuntimeProperty(name);
            if (propertyInfo == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("The property {0} is not found in class {1}.", name, type));
                return null;
            }
            var propertyTypeInfo = propertyInfo.PropertyType.GetTypeInfo();

            bool isCollection;
            bool isPrimitive;
            if (TypeIsPrimitive(propertyTypeInfo))
            {
                isCollection = false;
                isPrimitive = true;
            }
            else if (IsCollectionType(propertyTypeInfo))
            {
                isCollection = true;
                isPrimitive = TypeIsPrimitive(GetItemType(propertyTypeInfo));
            }
            else
            {
                isCollection = false;
                isPrimitive = false;
            }

            return new CustomMemberProvider(owner, type, name, propertyInfo, isPrimitive, isCollection);
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
            IObjectInstance obj = instance as IObjectInstance;
            if (obj == null)
                return null;

            ObjectProperty property = obj.LookupProperty(this);
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
            IObjectInstance obj = instance as IObjectInstance;
            if (obj == null)
                return;

            ObjectProperty property = obj.LookupProperty(this);
            if (property == null)
                return;

            property.SetValue(value);
        }

        public IXamlType TargetType
        {
            get { return _owner; }
        }

        public IXamlType Type
        {
            get
            {
                return
                    _isPrimitive || _isCollection
                        ? (IXamlType)new PrimitiveTypeProvider(_propertyInfo.PropertyType)
                        : CustomMetadataProvider.GetDependentType(_propertyInfo.PropertyType);
            }
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
