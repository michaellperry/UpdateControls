using System;
using System.ComponentModel;
using System.Windows.Threading;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    internal partial class DependentCollection<T>
    {
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        partial void TriggerUpdate(Dependent depCollection)
        {
            _dispatcher.BeginInvoke(new Action(delegate
            {
                using (NotificationGate.BeginOutbound())
                {
                    depCollection.OnGet();
                }
			}), System.Windows.Threading.DispatcherPriority.Background);
        }
    }

    public partial class ViewModelBase
    {
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        partial void FirePropertyChanged(string propertyName)
        {
            _dispatcher.BeginInvoke(new Action(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
