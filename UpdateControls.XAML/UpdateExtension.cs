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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace UpdateControls.XAML
{
    [MarkupExtensionReturnType(typeof(object))]
    public class UpdateExtension : MarkupExtension
    {
        private FrameworkElement _targetObject;
        private DependencyProperty _targetProperty;

        private ValueDependencyObject _valueObject;
        private Binding _binding = new Binding();

        public UpdateExtension(string path)
        {
            _valueObject = new ValueDependencyObject(path);
        }

		public string Path
		{
			get { return _valueObject.Path; }
			set { _valueObject.Path = value; }
		}

        [DefaultValue(BindingMode.TwoWay)]
        public BindingMode Mode
        {
            get { return _binding.Mode; }
            set { _binding.Mode = value; }
        }

		[DefaultValue(UpdateSourceTrigger.Default)]
		public UpdateSourceTrigger UpdateSourceTrigger
		{
			get { return _binding.UpdateSourceTrigger; }
            set { _binding.UpdateSourceTrigger = value; }
		}

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (target != null)
            {
                _targetProperty = target.TargetProperty as DependencyProperty;
                _targetObject = target.TargetObject as FrameworkElement;
                if (_targetObject != null && _targetProperty != null)
                {
                    // Register for notification when the data context changes.
                    _targetObject.DataContextChanged += DataContextChanged;
                    _valueObject.DataContext = _targetObject.DataContext;
                }
            }

            // Bind to the value dependency property.
            _binding.Source = _valueObject;
            _binding.Path = new PropertyPath("Value");
            return _binding.ProvideValue(serviceProvider);
        }

        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _valueObject.DataContext = _targetObject.DataContext;
        }
    }
}
