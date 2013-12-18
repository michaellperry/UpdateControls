using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;
using System.ComponentModel;

namespace UpdateControls.Test
{
    public class DataGridPersonViewModel : IEditableObject
    {
        private Person _person;
        private Person _copy;
        private ContactList _contactList;
        private static List<PrefixViewModel> _prefixes =
            new List<PrefixViewModel>
            {
		        new PrefixViewModel(PrefixID.None),
		        new PrefixViewModel(PrefixID.Mr),
		        new PrefixViewModel(PrefixID.Mrs),
		        new PrefixViewModel(PrefixID.Miss),
		        new PrefixViewModel(PrefixID.Dr)
            };

        public DataGridPersonViewModel(Person person, ContactList contactList)
        {
            _person = person;
            _copy = _person;
            _contactList = contactList;
        }

		public Person Person
		{
			get { return _person; }
		}

		public PrefixViewModel Prefix
		{
            get { return _prefixes.First(p => p.Prefix == _copy.Prefix); }
			set { _copy.Prefix = value.Prefix; }
		}

		public IEnumerable<PrefixViewModel> Prefixes
		{
			get { return _prefixes; }
		}

		public string First
		{
            get { return _copy.First; }
			set { _copy.First = value.Trim(); }
		}

		public string Last
		{
            get { return _copy.Last; }
			set { _copy.Last = value.Trim(); }
		}

		public string Gender
		{
            get { return _copy.Gender == GenderEnum.Male ? "Male" : "Female"; }
			set { _copy.Gender = value == "Male" ? GenderEnum.Male : GenderEnum.Female; }
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
            DataGridPersonViewModel that = obj as DataGridPersonViewModel;
			if (that == null)
				return false;
			return Object.Equals(this._person, that._person);
		}

		public override int GetHashCode()
		{
			return _person.GetHashCode();
		}

        #region IEditableObject Members

        public void BeginEdit()
        {
            _copy = new Person()
            {
                First = _person.First,
                Last = _person.Last,
                Prefix = _person.Prefix,
                Gender = _person.Gender
            };
        }

        public void CancelEdit()
        {
            _copy = _person;
        }

        public void EndEdit()
        {
            if (_copy != _person)
            {
                _person.First = _copy.First;
                _person.Last = _copy.Last;
                _person.Prefix = _copy.Prefix;
                _person.Gender = _copy.Gender;

                _copy = _person;
            }
        }

        #endregion
    }
}
