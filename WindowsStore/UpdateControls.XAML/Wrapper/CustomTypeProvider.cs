using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace UpdateControls.XAML.Wrapper
{
    class CustomTypeProvider : IXamlType
    {
        private Type _type;
        private Type _underlyingType;
        private Dictionary<string, CustomMemberProvider> _members = new Dictionary<string, CustomMemberProvider>();

        public CustomTypeProvider(Type type)
        {
            _type = type;
            _underlyingType = typeof(ObjectInstance<>).MakeGenericType(type);
        }

        public object ActivateInstance()
        {
            throw new NotImplementedException();
        }

        public void AddToMap(object instance, object key, object value)
        {
            throw new NotImplementedException();
        }

        public void AddToVector(object instance, object value)
        {
            throw new NotImplementedException();
        }

        public IXamlType BaseType
        {
            get { return null; }
        }

        public IXamlMember ContentProperty
        {
            get { return null; }
        }

        public object CreateFromString(string value)
        {
            throw new NotImplementedException();
        }

        public string FullName
        {
            get { return _type.FullName; }
        }

        public IXamlMember GetMember(string name)
        {
            CustomMemberProvider member;
            if (!_members.TryGetValue(name, out member))
            {
                member = CustomMemberProvider.For(this, _type, name);
                if (member != null)
                    _members.Add(name, member);
            }
            return member;
        }

        public bool IsArray
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBindable
        {
            get { return true; }
        }

        public bool IsCollection
        {
            get { return false; }
        }

        public bool IsConstructible
        {
            get { return false; }
        }

        public bool IsDictionary
        {
            get { return false; }
        }

        public bool IsMarkupExtension
        {
            get { return false; }
        }

        public IXamlType ItemType
        {
            get { throw new NotImplementedException(); }
        }

        public IXamlType KeyType
        {
            get { throw new NotImplementedException(); }
        }

        public void RunInitializer()
        {
        }

        public Type UnderlyingType
        {
            get { return _underlyingType; }
        }
    }
}
