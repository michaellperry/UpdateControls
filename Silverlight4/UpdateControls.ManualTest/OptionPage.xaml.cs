using System.Windows.Controls;
using System.Windows.Navigation;
using UpdateControls.XAML;

namespace UpdateControls.ManualTest
{
    public partial class OptionPage : Page
    {
        public OptionPage()
        {
            InitializeComponent();

            DataContext = ForView.Wrap(new OptionModel());
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

    }
}
