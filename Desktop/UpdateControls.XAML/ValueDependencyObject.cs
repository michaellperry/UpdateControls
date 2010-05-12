/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
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
            ((ValueDependencyObject)obj)._valueSource.OnValueChanged();
        }

        private ValueSource _valueSource;

        public ValueDependencyObject(string path, FrameworkElement targetObject, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
        {
            _valueSource = new ValueSource(
                path,
                targetObject,
                mode,
                updateSourceTrigger,
                OnUpdateValue,
                () => GetValue(_valueProperty),
                () => Dispatcher.BeginInvoke(new Action(_valueSource.TriggerUpdate)) );
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return _valueSource.ProvideValue(serviceProvider, this);
        }

        private void OnUpdateValue(object value)
        {
            // Put the property value into the bound source.
            if (value == null)
                value = DependencyProperty.UnsetValue;
            SetValue(_valueProperty, value);
        }
    }
}
