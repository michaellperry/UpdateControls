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

using System;

namespace UpdateControls.XAML.Test
{
    public class PersonListPresentation
    {
		private PersonList _personList;
        private PersonListNavigation _navigation;

        public PersonListPresentation(PersonList person, PersonListNavigation navigation)
        {
			_personList = person;
            _navigation = navigation;
        }

		public PersonList PersonList
		{
			get { return _personList; }
		}

        public PersonListNavigation Navigation
        {
            get { return _navigation; }
        }

        public string Title
        {
            get
            {
                string personDisplay = string.Empty;
                if (_navigation != null && _navigation.SelectedPerson != null)
                    personDisplay = _navigation.SelectedPerson.Display;
                return "Person - " + personDisplay;
            }
        }
    }
}
