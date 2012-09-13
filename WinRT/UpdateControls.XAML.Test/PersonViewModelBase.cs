using System;
using System.Linq;
using System.Collections.Generic;

namespace UpdateControls.XAML.Test
{
    public class PersonViewModelBase
	{
		private Person _person;
		private static List<PrefixViewModel> _prefixes = Enum.GetValues(typeof(PrefixID))
			.OfType<PrefixID>()
			.Select(p => new PrefixViewModel(p))
			.ToList();

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
			get { return _prefixes.First(p => p.Prefix == _person.Prefix); }
			set { if (value != null) _person.Prefix = value.Prefix; }
		}

		public IEnumerable<PrefixViewModel> Prefixes
		{
			get { return _prefixes; }
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

        public IEnumerable<GenderOption> GenderOptions
        {
            get
            {
                yield return new GenderOption(GenderEnum.Male);
                yield return new GenderOption(GenderEnum.Female);
            }
        }

        public GenderOption Gender
		{
            get { return new GenderOption(_person.Gender); }
			set { if (value != null) _person.Gender = value.Gender; }
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
