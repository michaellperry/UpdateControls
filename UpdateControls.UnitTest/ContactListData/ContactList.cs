using System.Collections.Generic;

namespace UpdateControls.UnitTest.ContactListData
{
    public class ContactList
    {
        private List<Contact> _contacts = new List<Contact>();
        private Independent _indContacts = new Independent();

        public void AddContact(Contact contact)
        {
            _indContacts.OnSet();
            _contacts.Add(contact);
        }

        public void DeleteContact(Contact contact)
        {
            _indContacts.OnSet();
            _contacts.Remove(contact);
        }

        public IEnumerable<Contact> Contacts
        {
            get { _indContacts.OnGet(); return _contacts; }
        }
    }
}
