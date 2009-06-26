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
		public Window1()
		{
			InitializeComponent();
			ContactList contactList = new ContactList();
			Person mike = contactList.NewPerson();
			mike.First = "Michael";
			mike.Last = "Perry";
			mike.Gender = GenderEnum.Male;
			Person jenny = contactList.NewPerson();
			jenny.First = "Jennifer";
			jenny.Last = "Perry";
			jenny.Gender = GenderEnum.Female;
			mike.Spouse = jenny;
			jenny.Spouse = mike;
			DataContext = ForView.Wrap(new ContactListViewModel(contactList, new ContactListNavigationModel()));
		}
	}
}
