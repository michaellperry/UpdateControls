using System;
using System.Reflection;
using System.Windows;

namespace UpdateControls.XAML
{
    public class ValueDependencyObject : DependencyObject
    {
        private static DependencyProperty _valueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(ValueDependencyObject),
            new PropertyMetadata(OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ValueDependencyObject)obj).OnValueChanged();
        }

        private string _path;
        private object _dataContext;
        private Independent _indDataContext = new Independent();
        private Dependent _depValue;

        private MethodInfo _getMethod;
        private MethodInfo _setMethod;
        private Dependent _depMethodInfo;

        public ValueDependencyObject(string path)
        {
            _path = path;
            _depMethodInfo = new Dependent(UpdateMethodInfo);
            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
            _depValue.OnGet();
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public object DataContext
        {
            get { _indDataContext.OnGet(); return _dataContext; }
            set { _indDataContext.OnSet(); _dataContext = value; }
        }

        private void UpdateMethodInfo()
        {
            _getMethod = null;
            _setMethod = null;

            _indDataContext.OnGet();
            if (_dataContext != null)
            {
                // Find the property identified by path.
                Type contextType = _dataContext.GetType();
                MemberInfo[] member = contextType.GetMember(_path);
                if (member != null && member.Length == 1)
                {
                    PropertyInfo propertyInfo = member[0] as PropertyInfo;
                    if (propertyInfo != null && propertyInfo.CanRead)
                    {
                        _getMethod = propertyInfo.GetGetMethod();
                        _setMethod = propertyInfo.GetSetMethod();
                    }
                }
            }
        }

        private void UpdateValue()
        {
            _depMethodInfo.OnGet();
            if (_getMethod != null)
            {
                object value = _getMethod.Invoke(_dataContext, null);
                if (value == null)
                    value = DependencyProperty.UnsetValue;
                SetValue(_valueProperty, value);
            }
        }

        private void ValueInvalidated()
        {
            Dispatcher.BeginInvoke(new Action(() => _depValue.OnGet()));
        }

        private void OnValueChanged()
        {
            if (_depValue.IsNotUpdating)
            {
                _depMethodInfo.OnGet();
                if (_setMethod != null)
                {
                    _setMethod.Invoke(_dataContext, new object[] { GetValue(_valueProperty) });
                }
            }
        }
    }
}
