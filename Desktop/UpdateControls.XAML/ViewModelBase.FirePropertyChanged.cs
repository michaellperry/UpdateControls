using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace UpdateControls.XAML
{
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
