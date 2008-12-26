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

		public Person Person
		{
			get { return _person; }
		}

        public string Title
        {
            get { return "Person - " + _person.Display; }
        }
    }
}
