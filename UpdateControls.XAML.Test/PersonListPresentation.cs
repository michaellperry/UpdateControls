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

using System.Windows.Input;
using System.Linq;

namespace UpdateControls.XAML.Test
{
    public class PersonListPresentation
    {
		private PersonList _personList;
        private PersonListNavigation _navigation;

        public PersonListPresentation(PersonList personList, PersonListNavigation navigation)
        {
			_personList = personList;
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

        public int SelectedPersonIndex
        {
            get { return _personList.People.IndexOf(_navigation.SelectedPerson); }
            set { _navigation.SelectedPerson = 0 <= value && value < _personList.People.Count ? _personList.People.ElementAt(value) : null; }
        }

        public ICommand AddPerson
        {
            get
            {
                return MakeCommand
                    .Do(() =>
                    {
                        Navigation.SelectedPerson = PersonList.NewPerson();
                    });
            }
        }

        public ICommand DeletePerson
        {
            get
            {
                return MakeCommand
                    .When(() => Navigation.SelectedPerson != null)
                    .Do(() =>
                    {
                        PersonList.DeletePerson(Navigation.SelectedPerson);
                    });
            }
        }
    }
}
