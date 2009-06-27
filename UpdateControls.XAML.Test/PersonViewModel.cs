using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.XAML.Test
{
	public class PersonViewModel
	{
		private Person _person;
		private ContactList _contactList;

		private PersonViewModel(Person person, ContactList contactList)
		{
			_person = person;
			_contactList = contactList;
		}

		public string First
		{
			get { return _person.First; }
			set { _person.First = value; }
		}

		public string Last
		{
			get { return _person.Last; }
			set { _person.Last = value; }
		}

		public string FullName
		{
			get { return _person.FullName; }
		}

		public string Gender
		{
			get { return _person.Gender == GenderEnum.Male ? "Male" : "Female"; }
			set { _person.Gender = value == "Male" ? GenderEnum.Male : GenderEnum.Female; }
		}

		public PersonViewModel Spouse
		{
			get { return PersonViewModel.Wrap(_person.Spouse, _contactList); }
			set { _person.Spouse = PersonViewModel.Unwrap(value); }
		}

		public IEnumerable<PersonViewModel> PotentialSpouses
		{
			get
			{
				// Return all people of the opposite gender.
				return _contactList.People.Where(p => p.Gender != _person.Gender).Select(p => PersonViewModel.Wrap(p, _contactList));
			}
		}

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            PersonViewModel that = obj as PersonViewModel;
            if (that == null)
                return false;
            bool objectEquals = Object.Equals(this._person, that._person);
            if (objectEquals)
                return true;
            else
                return false;
        }

		public override int GetHashCode()
		{
			return _person.GetHashCode();
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
				return viewModel._person;
		}
	}
}
