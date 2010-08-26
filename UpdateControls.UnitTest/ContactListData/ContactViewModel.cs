
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
    }
}
