using System.ComponentModel;
using System.Windows;
using System;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    internal partial class DependentCollection
    {
        partial void TriggerUpdate()
        {
            Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate
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
        partial void FirePropertyChanged(string propertyName)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}
