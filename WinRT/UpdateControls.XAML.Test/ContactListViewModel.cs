using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;

namespace UpdateControls.XAML.Test
{
    public class ContactListViewModel
	{
		private ContactList _contactList;
		private ContactListNavigationModel _navigation;

		public ContactListViewModel(ContactList contactList, ContactListNavigationModel navigation)
		{
			_contactList = contactList;
			_navigation = navigation;
		}

        public DataGridContactListViewModel DataGridVM
        {
            get { return new DataGridContactListViewModel(_contactList); }
        }

		public IEnumerable<PersonViewModel> People
		{
			get { return _contactList.People.Select(p => PersonViewModel.Wrap(p, _contactList)); }
		}

		public PersonViewModel SelectedPerson
		{
			get { return PersonViewModel.Wrap(_navigation.SelectedPerson, _contactList); }
			set { _navigation.SelectedPerson = PersonViewModel.Unwrap(value); }
		}

		public bool IsPersonSelected
		{
			get { return _navigation.SelectedPerson != null; }
		}

		public ICommand NewPerson
		{
			get
			{
				return MakeCommand
					.Do(() => _navigation.SelectedPerson = _contactList.NewPerson());
			}
		}

		public ICommand DeletePerson
		{
			get
			{
				return MakeCommand
					.When(() => _navigation.SelectedPerson != null)
					.Do(() => _contactList.DeletePerson(_navigation.SelectedPerson));
			}
		}
	}
}
