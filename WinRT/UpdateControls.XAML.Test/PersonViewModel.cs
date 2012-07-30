using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace UpdateControls.XAML.Test
{
    public class PersonViewModel : PersonViewModelBase, IDataErrorInfo
	{
		private ContactList _contactList;

		private PersonViewModel(Person person, ContactList contactList)
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

		public string Error
		{
			get { return null; }
		}

		public string this[string columnName]
		{
			get
			{
				if (columnName == "Last")
					return string.IsNullOrEmpty(this.Last) ? "Last name is required." : null;
				else if (columnName == "First")
					return string.IsNullOrEmpty(this.First) ? "First name is required." : null;
				else
					return null;
			}
		}
	}
}
