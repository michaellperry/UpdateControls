using System.Windows;
using System.Windows.Threading;

namespace UpdateControls.XAML
{
	public partial class MakeCommand
	{
        internal static Dispatcher GetDispatcher()
        {
            return Deployment.Current.Dispatcher;
        }
    }
}
