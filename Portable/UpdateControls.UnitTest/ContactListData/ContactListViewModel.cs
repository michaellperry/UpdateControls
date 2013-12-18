using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.UnitTest.ContactListData
{
    public class ContactListViewModel
    {
        private ContactList _contactList;
        private ContactListSortOrder _sortOrder;
        private Independent _indSortOrder = new Independent();

        private List<ContactViewModel> _contactViewModels;
        private Dependent _depContactViewModels;

        public delegate void NotifyCollectionChanged();
        public event NotifyCollectionChanged ContactsCollectionChanged;

        public ContactListViewModel(ContactList contactList)
        {
            _contactList = contactList;
            _depContactViewModels = new Dependent(UpdateContactViewModels);
            _depContactViewModels.Invalidated += new Action(_depContactViewModels_Invalidated);
        }

        void _depContactViewModels_Invalidated()
        {
            ContactsCollectionChanged();
        }

        public ContactListSortOrder SortOrder
        {
            get
            {
                _indSortOrder.OnGet();
                return _sortOrder;
            }
            set
            {
                if (_sortOrder != value) _indSortOrder.OnSet();
                _sortOrder = value;
            }
        }

        public IEnumerable<ContactViewModel> Contacts
        {
            get
            {
                _depContactViewModels.OnGet();
                return _contactViewModels;
            }
        }

        private void UpdateContactViewModels()
        {
            IEnumerable<Contact> contacts = _contactList.Contacts;
            if (SortOrder == ContactListSortOrder.FirstName)
                contacts = contacts.OrderBy(contact => contact.FirstName);
            else if (SortOrder == ContactListSortOrder.LastName)
                contacts = contacts.OrderBy(contact => contact.LastName);

            _contactViewModels = contacts
                .Select(contact => new ContactViewModel(contact))
                .ToList();
        }
    }
}
