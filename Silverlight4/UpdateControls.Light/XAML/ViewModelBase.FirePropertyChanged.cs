using System.ComponentModel;
using System.Windows;
using UpdateControls.XAML.Wrapper;
using System;

namespace UpdateControls.XAML
{
    internal partial class DependentCollection<T>
    {
        partial void TriggerUpdate(Dependent depCollection)
        {
            Deployment.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                using (NotificationGate.BeginOutbound())
                {
                    depCollection.OnGet();
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
