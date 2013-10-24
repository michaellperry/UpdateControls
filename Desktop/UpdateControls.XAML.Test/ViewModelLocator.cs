using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.XAML.Test
{
    public class ViewModelLocator : ViewModelLocatorBase
    {
        private ContactList _contactList;

        public ViewModelLocator()
        {
            // Create some test data just to get started.
            _contactList = new ContactList();
            Person mike = _contactList.NewPerson();
            mike.First = "Michael";
            mike.Last = "Perry";
            mike.Gender = GenderEnum.Male;
            Person jenny = _contactList.NewPerson();
            jenny.First = "Jennifer";
            jenny.Last = "Perry";
            jenny.Gender = GenderEnum.Female;
            Person.Marry(mike, jenny);
        }

        public object ContactList
        {
            get
            {
                return ViewModel(() => true ? null : new ContactListViewModel(_contactList, new ContactListNavigationModel()));
            }
        }
    }
}
