using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UpdateControls.Test;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UpdateControls.XAML.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var contactList = new ContactList();

            Person mike = contactList.NewPerson();
            mike.Prefix = PrefixID.Mr;
            mike.First = "Michael";
            mike.Last = "Perry";
            mike.Gender = GenderEnum.Male;
            Person jenny = contactList.NewPerson();
            jenny.Prefix = PrefixID.Mrs;
            jenny.First = "Jennifer";
            jenny.Last = "Perry";
            jenny.Gender = GenderEnum.Female;
            Person.Marry(mike, jenny);

            ContactListViewModel viewModel = new ContactListViewModel(contactList, new ContactListNavigationModel());
            DataContext = ForView.Wrap(viewModel);
        }
    }
}
