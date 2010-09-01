using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;

namespace UpdateControls.XAML.Test
{
    public class DataGridContactListViewModel
    {
        private ContactList _contactList;

        public DataGridContactListViewModel(ContactList contactList)
        {
            _contactList = contactList;
        }

        public IEnumerable<DataGridPersonViewModel> People
        {
            get { return _contactList.People.Select(p => new DataGridPersonViewModel(p, _contactList)); }
        }
    }
}
