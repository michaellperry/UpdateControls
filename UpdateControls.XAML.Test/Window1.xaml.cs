using System;
using System.Windows;

namespace UpdateControls.XAML.Test
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
        private ContactList _contactList;

        public Window1()
            : this(PopulateContactList())
        {
        }

        public Window1(ContactList contactList)
		{
            InitializeComponent();

            // Save the contact list so I can open a new window with it.
            _contactList = contactList;

            // Give this window its own unique navigation, but share
            // the contact list. Wrap both in a view model, and wrap the
            // view model with Update Controls.
            DataContext = ForView.Wrap(new ContactListViewModel(_contactList, new ContactListNavigationModel()));
		}

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            // Show a new window that shares the same data model.
            new Window1(_contactList).Show();
        }

        private static ContactList PopulateContactList()
        {
            // Create some test data just to get started.
            ContactList contactList = new ContactList();
            Person mike = contactList.NewPerson();
            mike.First = "Michael";
            mike.Last = "Perry";
            mike.Gender = GenderEnum.Male;
            Person jenny = contactList.NewPerson();
            jenny.First = "Jennifer";
            jenny.Last = "Perry";
            jenny.Gender = GenderEnum.Female;
            Person.Marry(mike, jenny);
            return contactList;
        }
    }
}
