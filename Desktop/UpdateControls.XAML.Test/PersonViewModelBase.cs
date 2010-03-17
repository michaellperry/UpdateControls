using System;

namespace UpdateControls.XAML.Test
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

		public ISpouseViewModel Spouse
		{
			get { return SpouseViewModel.Wrap(_person.Spouse); }
			set { Person.Marry(_person, SpouseViewModel.Unwrap(value)); }
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
