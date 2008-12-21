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
using System.Collections;

namespace UpdateControls.XAML
{
    [MarkupExtensionReturnType(typeof(object))]
    public class UpdateExtension : MarkupExtension
    {
        private string _path;
        private BindingMode _mode;
        private UpdateSourceTrigger _updateSourceTrigger;

        public UpdateExtension(string path)
        {
            _path = path;
        }

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

        [DefaultValue(BindingMode.TwoWay)]
        public BindingMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

		[DefaultValue(UpdateSourceTrigger.Default)]
		public UpdateSourceTrigger UpdateSourceTrigger
		{
            get { return _updateSourceTrigger; }
            set { _updateSourceTrigger = value; }
		}

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (target != null)
            {
                DependencyProperty targetProperty = target.TargetProperty as DependencyProperty;
                FrameworkElement targetObject = target.TargetObject as FrameworkElement;
                if (targetObject != null && targetProperty != null)
                {
                    if (typeof(IEnumerable).Equals(targetProperty.PropertyType))
                    {
                        ValueObservableCollection collection = new ValueObservableCollection(_path, targetObject, targetProperty);
                        return collection.ProvideValue(serviceProvider);
                    }
                    else
                    {
                        ValueDependencyObject valueObject = new ValueDependencyObject(_path, targetObject, targetProperty);
                        return valueObject.ProvideValue(serviceProvider);
                    }
                }
            }

            return this;
        }
    }
}
