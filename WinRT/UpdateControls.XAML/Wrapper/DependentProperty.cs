using System.Reflection;
using UpdateControls.Fields;
using System;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    class DependentProperty
    {
        private static readonly object[] EmptyIndexer = new object[0];

        private readonly DynamicDependentWrapper _wrapper;
        private readonly object _wrappedObject;
        private readonly PropertyInfo _propertyInfo;

        private object _value;
        private Dependent _depValue;
        private bool _initialized = false;
        
        public DependentProperty(DynamicDependentWrapper wrapper, object wrappedObject, PropertyInfo propertyInfo)
        {
            _wrapper = wrapper;
            _wrappedObject = wrappedObject;
            _propertyInfo = propertyInfo;

            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
            _depValue.OnGet();
        }

        public object GetValue()
        {
            return _value;
        }

        public void SetValue(object value)
        {
            var affectedSet = AffectedSet.Begin();

            try
            {
                _propertyInfo.SetValue(_wrappedObject, value, EmptyIndexer);
            }
            finally
            {
                if (affectedSet != null)
                {
                    foreach (var dependent in affectedSet.End())
                        dependent.OnGet();
                }
            }
        }

        private void UpdateValue()
        {
            object newValue = _propertyInfo.GetValue(_wrappedObject, EmptyIndexer);
            if (_initialized)
            {
                if ((!Object.Equals(newValue, _value)))
                {
                    _value = newValue;
                    _wrapper.FirePropertyChanged(_propertyInfo.Name);
                }
            }
            else
            {
                _value = newValue;
                _initialized = true;
            }
        }

        private void ValueInvalidated()
        {
            if (!AffectedSet.CaptureDependent(_depValue))
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
                {
                    _depValue.OnGet();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}
