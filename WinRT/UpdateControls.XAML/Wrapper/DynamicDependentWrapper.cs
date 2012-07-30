using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.ComponentModel;

namespace UpdateControls.XAML.Wrapper
{
    class DynamicDependentWrapper : DynamicObject, INotifyPropertyChanged
    {
        private readonly object _wrappedObject;

        private Dictionary<string, DependentProperty> _propertyByName = new Dictionary<string, DependentProperty>();

        public DynamicDependentWrapper(object wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            DependentProperty dependentProperty = GetDependentProperty(binder.Name);
            if (dependentProperty == null)
            {
                result = null;
                return false;
            }

            result = dependentProperty.GetValue();
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            DependentProperty dependentProperty = GetDependentProperty(binder.Name);
            if (dependentProperty == null)
                return false;

            dependentProperty.SetValue(value);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private DependentProperty GetDependentProperty(string propertyName)
        {
            DependentProperty dependentProperty;
            if (!_propertyByName.TryGetValue(propertyName, out dependentProperty))
            {
                PropertyInfo propertyInfo = _wrappedObject.GetType().GetProperty(propertyName);
                if (propertyInfo == null)
                    return null;

                dependentProperty = new DependentProperty(this, _wrappedObject, propertyInfo);
                _propertyByName.Add(propertyName, dependentProperty);
            }
            return dependentProperty;
        }
    }
}
