using System;

namespace UpdateControls.UnitTest.ContactListData
{
    public class ContactViewModel
    {
        private Contact _contact;

        public ContactViewModel(Contact contact)
        {
            _contact = contact;
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", _contact.FirstName, _contact.LastName);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            ContactViewModel that = obj as ContactViewModel;
            if (that == null)
                return false;
            return _contact == that._contact;
        }

        public override int GetHashCode()
        {
            return _contact.GetHashCode();
        }
    }
}
