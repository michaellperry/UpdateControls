using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UpdateControls.XAML
{
    internal abstract class DependentPropertyBase
    {
        public abstract object Value { get; }
    }

    internal class DependentProperty<T> : DependentPropertyBase
    {
        private Dependent _depValue;
        private T _value;

        public DependentProperty(Action firePropertyChanged, Func<T> getMethod)
        {
            _depValue = new Dependent(() => _value = getMethod());
            _depValue.Invalidated += firePropertyChanged;
        }

        public override object Value
        {
            get { _depValue.OnGet(); return _value; }
        }
    }

    public partial class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IDictionary<string, DependentPropertyBase> _dependentPropertyByName = new Dictionary<string, DependentPropertyBase>();

        protected T GetProperty<T>(Func<T> getMethod)
        {
            string caller = new StackFrame(1).GetMethod().Name;
            if (!caller.StartsWith("get_"))
                throw new ArgumentException("Only call GetProperty from a property getter.");
            return GetProperty<T>(caller.Substring(4), getMethod);
        }

        protected T GetProperty<T>(string propertyName, Func<T> getMethod)
        {
            DependentPropertyBase property;
            if (!_dependentPropertyByName.TryGetValue(propertyName, out property))
            {
                property = new DependentProperty<T>(() => FirePropertyChanged(propertyName), getMethod);
                _dependentPropertyByName.Add(propertyName, property);
            }
            return (T)property.Value;
        }

        partial void FirePropertyChanged(string propertyName);

        /* Silverlight
        private void FirePropertyChanged(string propertyName)
        {
            App.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }
        */
    }
}
