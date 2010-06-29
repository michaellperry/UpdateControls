using System;
using System.ComponentModel;
using System.Windows.Threading;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    internal partial class DependentCollection
    {
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        partial void TriggerUpdate()
        {
            _dispatcher.BeginInvoke(new Action(delegate
            {
                using (NotificationGate.BeginOutbound())
                {
                    _depCollection.OnGet();
                }
            }));
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
            }));
        }
    }
}
