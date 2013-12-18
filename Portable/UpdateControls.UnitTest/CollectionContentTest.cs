using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.UnitTest.ContactListData;
using System.Linq;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class CollectionContentTest
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
        public void WhenContactAddedShouldNotifyCollectionChanged()
        {
            ContactViewModel firstByDefaultOrder = _viewModel.Contacts.First();
            _contactList.AddContact(new Contact() { FirstName = "Martin", LastName = "Fowler" });

            Assert.AreEqual(1, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenContactDeletedShouldNotifyCollectionChanged()
        {
            ContactViewModel firstByDefaultOrder = _viewModel.Contacts.First();
            _contactList.DeleteContact(_contactList.Contacts.First());

            Assert.AreEqual(1, _collectionChangedCount);
        }
    }
}
