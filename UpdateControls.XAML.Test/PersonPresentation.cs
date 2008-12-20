using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UpdateControls.XAML.Test
{
    public class PersonPresentation
    {
		private Person _person;

        public PersonPresentation(Person person)
        {
			_person = person;
        }

        public string FirstName
        {
            get { return _person.FirstName; }
            set { _person.FirstName = value; }
        }

        public string LastName
        {
            get { return _person.LastName; }
            set { _person.LastName = value; }
        }

        public int DisplayStrategy
        {
            get { return _person.DisplayStrategy; }
            set { _person.DisplayStrategy = value; }
        }

        public string FirstLast
        {
            get { return _person.FirstName + " " + _person.LastName; }
        }

        public string LastFirst
        {
            get { return _person.LastName + ", " + _person.FirstName; }
        }

        public string Title
        {
            get { return "Person - " + (DisplayStrategy == 0 ? FirstLast : LastFirst); }
        }
    }
}
