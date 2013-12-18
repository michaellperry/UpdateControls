using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.UnitTest.ContactListData;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class DynamicSortOrderTest
    {
        private ContactList _contactList;
        private ContactListViewModel _viewModel;
        private int _collectionChangedCount;

        [TestInitialize]
        public void Initialize()
        {
            _contactList = new ContactList();
            _viewModel = new ContactListViewModel(_contactList);
            _contactList.AddContact(new Contact() { FirstName = "Michael", LastName = "Perry" });
            _contactList.AddContact(new Contact() { FirstName = "Ada", LastName = "Lovelace" });
            _contactList.AddContact(new Contact() { FirstName = "Charles", LastName = "Babbage" });

            _collectionChangedCount = 0;
            _viewModel.ContactsCollectionChanged +=
                delegate
                {
                    _collectionChangedCount++;
                };
        }

        [TestMethod]
        public void InitiallyNoEventFired()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            Assert.AreEqual(0, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenOrderByFirstNameAndFirstNameChangesShouldNotify()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            _contactList.Contacts.First().FirstName = "George";

            Assert.AreEqual(1, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenOrderByLastNameAndFirstNameChangesShouldNotNotify()
        {
            _viewModel.SortOrder = ContactListSortOrder.LastName;
            ContactViewModel firstByLastName = _viewModel.Contacts.First();
            _contactList.Contacts.First().FirstName = "George";

            Assert.AreEqual(0, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenSortOrderChangedShouldNotify()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            _contactList.Contacts.First().FirstName = "George";
            ContactViewModel firstByFirstNameAgain = _viewModel.Contacts.First();

            _viewModel.SortOrder = ContactListSortOrder.LastName;

            Assert.AreEqual(2, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenSortOrderChangesAndFirstNameChangesShouldNoLongerNotify()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            _contactList.Contacts.First().FirstName = "George";
            ContactViewModel firstByFirstNameAgain = _viewModel.Contacts.First();

            _viewModel.SortOrder = ContactListSortOrder.LastName;
            var firstByLastName = _viewModel.Contacts.First();

            _contactList.Contacts.First().FirstName = "Charles";

            Assert.AreEqual(2, _collectionChangedCount);
        }
    }
}
