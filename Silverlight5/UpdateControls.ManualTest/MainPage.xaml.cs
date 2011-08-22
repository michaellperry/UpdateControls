using System.Windows;
using System.Windows.Controls;
using UpdateControls.XAML;

namespace UpdateControls.ManualTest
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FamilyViewModel viewModel = new FamilyViewModel(new Family());
            var wrapper = ForView.Wrap(viewModel);
            DataContext = wrapper;
        }
    }
}
