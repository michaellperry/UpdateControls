using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.Fields;
using UpdateControls.XAML;

namespace UpdateControls.XAML.Test
{
    public class ContactListViewModel : ViewModelBase
	{
		private ContactList _contactList;
		private ContactListNavigationModel _navigation;

        private static List<PrefixViewModel> _prefixes = Enum.GetValues(typeof(PrefixID))
            .OfType<PrefixID>()
            .Select(p => new PrefixViewModel(p))
            .ToList();

        public static IEnumerable<PrefixViewModel> AllPrefixes { get { return _prefixes; } }

		public ContactListViewModel(ContactList contactList, ContactListNavigationModel navigation)
		{
			_contactList = contactList;
			_navigation = navigation;
		}

		public IEnumerable<PersonViewModel> People
		{
			get { return GetCollection(() => _contactList.People.Select(p => PersonViewModel.Wrap(p, _contactList))); }
		}

		public PersonViewModel SelectedPerson
		{
			get { return Get(() => PersonViewModel.Wrap(_navigation.SelectedPerson, _contactList)); }
			set { _navigation.SelectedPerson = PersonViewModel.Unwrap(value); }
		}

		public bool IsPersonSelected
		{
			get { return Get(() => _navigation.SelectedPerson != null); }
		}

        public IEnumerable<PrefixViewModel> Prefixes
        {
            get { return GetCollection(() => _prefixes); }
        }

        public IEnumerable<GenderEnum> GenderOptions
        {
            get
            {
                yield return GenderEnum.Male;
                yield return GenderEnum.Female;
            }
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
