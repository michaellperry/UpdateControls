/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<PathSegment> _segments;
        private object _dataContext;

        private FrameworkElement _targetObject;
        private DependencyProperty _targetProperty;
        private Binding _binding = new Binding();

        private Independent _indDataContext = new Independent();
        private Dependent _depValue;

        private Dependent _depMethodInfo;
		private Type _previousDataContextType = null;

        public ValueDependencyObject(string path, FrameworkElement targetObject, DependencyProperty targetProperty)
        {
			// Parse the dotted identifier syntax.
			_segments = (from identifier in path.Split('.')
				select new PathSegment(identifier)).ToList();

            _targetObject = targetObject;
            _targetProperty = targetProperty;

            _depMethodInfo = new Dependent(UpdateMethodInfo);
            _depValue = new Dependent(UpdateValue);
            _depValue.Invalidated += ValueInvalidated;
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
            _indDataContext.OnGet();
            if (_dataContext != null)
            {
                // Get the method info of each segment in the path.
				Type contextType = _dataContext.GetType();
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
			object value = _dataContext;
			foreach (PathSegment segment in _segments)
			{
				// Get the value of each property in the chain.
				if (segment.CanGet)
					value = segment.Get(value);
				else
					return;
			}

			// If we made it to the end, put the property value into the bound source.
			if (value == null)
				value = DependencyProperty.UnsetValue;
			SetValue(_valueProperty, value);
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
				object value = _dataContext;
				int count = _segments.Count;
				foreach (PathSegment segment in _segments)
				{
					// Short circuit path traversal on null.
					if (value == null)
						return;

					--count;
					if (count == 0)
					{
						// When we reach the final segment, set the property value.
						if (segment.CanSet)
							segment.Set(value, GetValue(_valueProperty));
					}
					else
					{
						// Follow each leading segment down the property chain.
						if (segment.CanGet)
							value = segment.Get(value);
						else
							return;
					}
				}
			}
		}
    }
}
