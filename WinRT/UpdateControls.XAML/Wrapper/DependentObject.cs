using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    internal interface IDependentObject
    {
        object GetWrappedObject();
        bool Equals(object obj);
        int GetHashCode();
        DependentProperty GetDependentProperty(CustomMemberProvider member);
        string ToString();
        void FirePropertyChanged(string propertyName);
    }
    class DependentObject<T> : IDependentObject, INotifyPropertyChanged, INotifyDataErrorInfo, IEditableObject
    {
        private readonly T _wrappedObject;

        private Dictionary<CustomMemberProvider, DependentProperty> _propertyByName = new Dictionary<CustomMemberProvider, DependentProperty>();

        public DependentObject(T wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            DependentObject<T> that = obj as DependentObject<T>;
            if (that == null)
                return false;
            return Object.Equals(this._wrappedObject, that._wrappedObject);
        }

        public override int GetHashCode()
        {
            return _wrappedObject.GetHashCode();
        }

        public override string ToString()
        {
            return _wrappedObject.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public object GetWrappedObject()
        {
            return _wrappedObject;
        }

        public DependentProperty GetDependentProperty(CustomMemberProvider provider)
        {
            DependentProperty dependentProperty;
            if (!_propertyByName.TryGetValue(provider, out dependentProperty))
            {
                PropertyInfo propertyInfo = _wrappedObject.GetType().GetRuntimeProperty(provider.Name);
                if (propertyInfo == null)
                    return null;

                dependentProperty = new DependentProperty(this, _wrappedObject, propertyInfo, provider);
                _propertyByName.Add(provider, dependentProperty);
            }
            return dependentProperty;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            var errors = _wrappedObject as INotifyDataErrorInfo;
            if (errors != null)
                return errors.GetErrors(propertyName);
            else
                return Enumerable.Empty<object>();
        }

        public bool HasErrors
        {
            get
            {
                var errors = _wrappedObject as INotifyDataErrorInfo;
                if (errors != null)
                    return errors.HasErrors;
                else
                    return false;
            }
        }

        public void BeginEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.BeginEdit();
        }

        public void CancelEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.CancelEdit();
        }

        public void EndEdit()
        {
            IEditableObject wrappedObject = _wrappedObject as IEditableObject;
            if (wrappedObject != null)
                wrappedObject.EndEdit();
        }
    }
}
