using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Test
{
    public class PersonViewModel : PersonViewModelBase
	{
		private ContactList _contactList;

		public PersonViewModel(Person person, ContactList contactList)
			: base(person)
		{
			_contactList = contactList;
		}

		public IEnumerable<ISpouseViewModel> PotentialSpouses
		{
			get
			{
                return
                    // Return all people of the opposite gender.
                    // Include an option to be unmarried.
                    new List<Person>() { null }
                    .Union(_contactList.People
                        .Where(p => p.Gender != Person.Gender)
                    )
                    .Select(p => (ISpouseViewModel)SpouseViewModel.Wrap(p));
			}
		}

		public static PersonViewModel Wrap(Person person, ContactList contactList)
		{
			if (person == null)
				return null;
			else
				return new PersonViewModel(person, contactList);
		}

		public static Person Unwrap(PersonViewModel viewModel)
		{
			if (viewModel == null)
				return null;
			else
				return viewModel.Person;
		}
    }
}
