using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace UpdateControls.XAML.Test
{
	public class PersonViewModel : IDataErrorInfo
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

		public SpouseViewModel Spouse
		{
			get { return SpouseViewModel.Wrap(_person.Spouse); }
			set { Person.Marry(_person, SpouseViewModel.Unwrap(value)); }
		}

		public IEnumerable<SpouseViewModel> PotentialSpouses
		{
			get
			{
                return
                    // Return all people of the opposite gender.
                    // Include an option to be unmarried.
                    new List<Person>() { null }
                    .Union(_contactList.People
                        .Where(p => p.Gender != _person.Gender)
                    )
                    .Select(p => SpouseViewModel.Wrap(p));
			}
		}

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            PersonViewModel that = obj as PersonViewModel;
            if (that == null)
                return false;
            return Object.Equals(this._person, that._person);
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
