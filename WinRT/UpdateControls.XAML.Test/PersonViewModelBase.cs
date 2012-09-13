using System;
using System.Linq;
using System.Collections.Generic;

namespace UpdateControls.XAML.Test
{
    public class PersonViewModelBase : ViewModelBase
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
			get { return Get(() => ContactListViewModel.AllPrefixes.First(p => p.Prefix == _person.Prefix)); }
			set { _person.Prefix = value.Prefix; }
		}

		public string First
		{
			get { return Get(() => _person.First); }
			set { _person.First = value.Trim(); }
		}

		public string Last
		{
			get { return Get(() => _person.Last); }
			set { _person.Last = value.Trim(); }
		}

		public string FullName
		{
			get { return Get(() => _person.FullName); }
		}

        public GenderEnum Gender
		{
            get { return Get(() => _person.Gender); }
			set { _person.Gender = value; }
		}

		public ISpouseViewModel Spouse
		{
			get { return Get(() => SpouseViewModel.Wrap(_person.Spouse)); }
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
