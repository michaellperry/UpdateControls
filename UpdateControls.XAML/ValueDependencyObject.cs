using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

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

        private FrameworkElement _targetObject;
        private DependencyProperty _targetProperty;
        private Binding _binding = new Binding();

        private Independent _indDataContext = new Independent();
        private Dependent _depValue;

        private MethodInfo _getMethod;
        private MethodInfo _setMethod;
        private Dependent _depMethodInfo;

        public ValueDependencyObject(string path, FrameworkElement targetObject, DependencyProperty targetProperty)
        {
            _path = path;
            _targetObject = targetObject;
            _targetProperty = targetProperty;

            _depMethodInfo = new Dependent(UpdateMethodInfo);
            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public BindingMode Mode
        {
            set { _binding.Mode = value; }
        }

        public UpdateSourceTrigger UpdateSourceTrigger
        {
            set { _binding.UpdateSourceTrigger = value; }
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            // Register for notification when the data context changes.
            _targetObject.DataContextChanged += DataContextChanged;
            _dataContext = _targetObject.DataContext;

            // Initialize the property value.
            _depValue.OnGet();

            // Bind to the value dependency property.
            _binding.Source = this;
            _binding.Path = new PropertyPath("Value");
            return _binding.ProvideValue(serviceProvider);
        }

        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _indDataContext.OnSet();
            _dataContext = _targetObject.DataContext;
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
