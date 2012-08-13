using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    class CustomMemberProvider : IXamlMember
    {
        private readonly Type _type;
        private readonly string _name;
        private readonly PropertyInfo _propertyInfo;
        
        public CustomMemberProvider(Type type, string name)
        {
            _name = name;
            _type = type;

            _propertyInfo = _type.GetRuntimeProperty(_name);
        }

        public object GetValue(object instance)
        {
            throw new NotImplementedException();
        }

        public bool IsAttachable
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDependencyProperty
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
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
            get { throw new NotImplementedException(); }
        }
    }
}
