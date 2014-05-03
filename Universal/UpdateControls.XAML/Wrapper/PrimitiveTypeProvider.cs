using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Markup;

namespace UpdateControls.XAML.Wrapper
{
    public class PrimitiveTypeProvider : IXamlType
    {
        private readonly Type _type;

        public PrimitiveTypeProvider(Type type)
        {
            _type = type;            
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
            get { throw new NotImplementedException(); }
        }

        public IXamlMember ContentProperty
        {
            get { throw new NotImplementedException(); }
        }

        public object CreateFromString(string value)
        {
            throw new NotImplementedException();
        }

        public string FullName
        {
            get { throw new NotImplementedException(); }
        }

        public IXamlMember GetMember(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsArray
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBindable
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCollection
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsConstructible
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDictionary
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMarkupExtension
        {
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
        }

        public Type UnderlyingType
        {
            get { return _type; }
        }
    }
}
