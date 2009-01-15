using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace UpdateControls.XAML
{
    class ValueSource
    {
        private List<PathSegment> _segments;
        private WeakReference _targetObject;
        private Action<object> _onUpdateValue;
        private Func<object> _onGetValue;
        private Binding _binding = new Binding();

		private WeakReference _dataContext = new WeakReference(null);
		private WeakReference _sourceObject = new WeakReference(null);

        private Independent _indDataContext = new Independent();
        private Dependent _depSourceObject;
        private Dependent _depValue;

        private Dependent _depMethodInfo;
        private Type _previousDataContextType = null;

        public ValueSource(string path, FrameworkElement targetObject, BindingMode mode, UpdateSourceTrigger updateSourceTrigger, Action<object> onUpdateValue, Func<object> onGetValue, Action valueInvalidated)
        {
            // Parse the dotted identifier syntax.
            _segments = (from identifier in path.Split('.')
                         select new PathSegment(identifier)).ToList();

            _targetObject = new WeakReference(targetObject);
            _binding.Mode = mode;
            _binding.UpdateSourceTrigger = updateSourceTrigger;
            _onUpdateValue = onUpdateValue;
            _onGetValue = onGetValue;

            _depSourceObject = new Dependent(UpdateSourceObject);
            _depMethodInfo = new Dependent(UpdateMethodInfo);
            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += valueInvalidated;
        }

        public Binding Binding
        {
            get { return _binding; }
        }

        public object ProvideValue(IServiceProvider serviceProvider, object source)
        {
            // Register for notification when the data context changes.
			FrameworkElement targetObject = (FrameworkElement)_targetObject.Target;
			if (targetObject != null)
			{
				targetObject.DataContextChanged += DataContextChanged;
				_dataContext.Target = targetObject.DataContext;
			}

            // Initialize the property value.
            _depValue.OnGet();

            // Bind to the value dependency property.
            _binding.Source = source;
            _binding.Path = new PropertyPath("Value");
            return _binding.ProvideValue(serviceProvider);
        }

        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
			FrameworkElement targetObject = (FrameworkElement)_targetObject.Target;
			if (targetObject != null)
			{
				_indDataContext.OnSet();
				_dataContext.Target = targetObject.DataContext;
			}
        }

        private void UpdateSourceObject()
        {
            _indDataContext.OnGet();
            // See if the data context is a DataSourceProvider.
            DataSourceProvider dataSourceProvider = _dataContext.Target as DataSourceProvider;
            if (dataSourceProvider != null)
                _sourceObject.Target = dataSourceProvider.Data;
            else
                _sourceObject.Target = _dataContext.Target;
        }

        private void UpdateMethodInfo()
        {
            _depSourceObject.OnGet();
			object sourceObject = _sourceObject.Target;
            if (sourceObject != null)
            {
                // Get the method info of each segment in the path.
                Type contextType = sourceObject.GetType();
                if (!Object.Equals(contextType, _previousDataContextType))
                {
                    _previousDataContextType = contextType;
                    foreach (PathSegment segment in _segments)
                        contextType = segment.CacheMethodInfo(contextType);
                }
            }
        }

        private void UpdateValue()
        {
            _depMethodInfo.OnGet();
            object value = _sourceObject.Target;
            foreach (PathSegment segment in _segments)
            {
                if (value == null)
                    break;

                // Get the value of each property in the chain.
                if (segment.CanGet)
                    value = segment.Get(value);
                else
                    return;
            }

            _onUpdateValue(value);
        }

        public void TriggerUpdate()
        {
            _depValue.OnGet();
        }

        public void OnValueChanged()
        {
            if (_depValue.IsNotUpdating)
            {
                _depMethodInfo.OnGet();
                object context = _sourceObject.Target;
                int count = _segments.Count;
                foreach (PathSegment segment in _segments)
                {
                    // Short circuit path traversal on null.
                    if (context == null)
                        return;

                    --count;
                    if (count == 0)
                    {
                        // When we reach the final segment, set the property value.
                        if (segment.CanSet)
                            segment.Set(context, _onGetValue());
                    }
                    else
                    {
                        // Follow each leading segment down the property chain.
                        if (segment.CanGet)
                            context = segment.Get(context);
                        else
                            return;
                    }
                }
            }
        }

        public object Value
        {
            get
            {
                _depValue.OnGet();
                return _onGetValue();
            }
        }
    }
}
