using System;
using System.Windows;

namespace UpdateControls.XAML.Test
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
        public Window1()
        {
            InitializeComponent();
		}

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            // Show a new window that shares the same data model.
            new Window1().Show();
        }
    }
}
