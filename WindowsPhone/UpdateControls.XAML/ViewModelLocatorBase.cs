using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UpdateControls.XAML.Wrapper;
using System.Diagnostics;

namespace UpdateControls.XAML
{
    public class ViewModelLocatorBase : INotifyPropertyChanged
    {
        private class ViewModelContainer : IUpdatable
        {
            private Dependent _dependent;
            private object _viewModel;
            private Action _firePropertyChanged;

            public ViewModelContainer(Action firePropertyChanged, Func<object> constructor)
            {
                _firePropertyChanged = firePropertyChanged;
                _dependent = new Dependent(() => _viewModel = ForView.Wrap(constructor()));
                _dependent.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
            }

            public object ViewModel
            {
                get { _dependent.OnGet(); return _viewModel; }
            }

            public void UpdateNow()
            {
                _firePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IDictionary<string, ViewModelContainer> _containerByName = new Dictionary<string, ViewModelContainer>();

        public object ViewModel(Func<object> constructor)
        {
            string caller = new StackFrame(1).GetMethod().Name;
            if (!caller.StartsWith("get_"))
                throw new ArgumentException("Only call ViewModel from a property getter.");
            string propertyName = caller.Substring(4);

            ForView.Initialize();
            ViewModelContainer container;
            if (!_containerByName.TryGetValue(propertyName, out container))
            {
                container = new ViewModelContainer(() => FirePropertyChanged(propertyName), constructor);
                _containerByName.Add(propertyName, container);
            }
            return container.ViewModel;
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
