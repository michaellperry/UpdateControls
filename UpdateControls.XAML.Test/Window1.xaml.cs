using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UpdateControls.XAML.Test
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
        PersonList _personList;

        // Create a new window with a new person list.
        public Window1() : this(new PersonList())
        {
        }

        public Window1(PersonList personList)
        {
            InitializeComponent();
            _personList = personList;
            DataContext = new PersonListPresentation(_personList, new PersonListNavigation());
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            // Create a new window using the same person list.
            new Window1(_personList).Show();
        }
    }
}
