using System;
using System.Linq;
using System.Collections.Generic;

namespace UpdateControls.Test
{
    public class PersonViewModelBase
	{
		private Person _person;

		public PersonViewModelBase(Person person)
		{
			_person = person;
		}

		public Person Person
		{
			get { return _person; }
		}

		public PrefixViewModel Prefix
		{
			get { return ContactListViewModel.AllPrefixes.First(p => p.Prefix == _person.Prefix); }
			set { _person.Prefix = value.Prefix; }
		}

		public string First
		{
			get { return _person.First; }
			set { _person.First = value.Trim(); }
		}

		public string Last
		{
			get { return _person.Last; }
			set { _person.Last = value.Trim(); }
		}

		public string FullName
		{
			get { return _person.FullName; }
		}

        public GenderEnum Gender
		{
            get { return _person.Gender; }
			set { _person.Gender = value; }
		}

		public ISpouseViewModel Spouse
		{
			get { return SpouseViewModel.Wrap(_person.Spouse); }
			set { if (value != null) Person.Marry(_person, value.Spouse); }
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
	}
}
