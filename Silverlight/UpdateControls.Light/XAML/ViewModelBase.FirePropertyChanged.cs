using System.ComponentModel;
using System.Windows;

namespace UpdateControls.XAML
{
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
