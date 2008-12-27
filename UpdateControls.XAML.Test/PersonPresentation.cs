/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/


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
