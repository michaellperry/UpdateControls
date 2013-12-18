using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.XAML;
using UpdateControls.Test;

namespace UpdateControls.Test.App
{
    public class ViewModelLocator : ViewModelLocatorBase
    {
        private ContactList _contactList;
        private ContactListNavigationModel _navigation;

        public ViewModelLocator()
        {
            _contactList = new ContactList();
            _navigation = new ContactListNavigationModel();

            Person mike = _contactList.NewPerson();
            mike.Prefix = PrefixID.Mr;
            mike.First = "Michael";
            mike.Last = "Perry";
            mike.Gender = GenderEnum.Male;
            Person jenny = _contactList.NewPerson();
            jenny.Prefix = PrefixID.Mrs;
            jenny.First = "Jennifer";
            jenny.Last = "Perry";
            jenny.Gender = GenderEnum.Female;
            Person.Marry(mike, jenny);
        }

        public object Main
        {
            get
            {
                return ViewModel(() => new ContactListViewModel(
                    _contactList,
                    _navigation));
            }
        }

        public object PersonDetail
        {
            get
            {
                return ViewModel(() => _navigation.SelectedPerson == null
                    ? null
                    : new PersonViewModel(
                        _navigation.SelectedPerson,
                        _contactList));
            }
        }
    }
}
