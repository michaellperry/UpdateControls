using System.Windows.Threading;

namespace UpdateControls.XAML
{
	public partial class MakeCommand
	{
        internal static Dispatcher GetDispatcher()
        {
            return Dispatcher.CurrentDispatcher;
        }
    }
}
