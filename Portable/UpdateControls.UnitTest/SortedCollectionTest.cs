using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.UnitTest.ContactListData;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class SortedCollectionTest
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
        public void WhenSortByFirstNameShouldSortViewModel()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;

            ContactViewModel contactViewModel = _viewModel.Contacts.First();
            Assert.AreEqual("Ada Lovelace", contactViewModel.FullName);
        }

        [TestMethod]
        public void WhenViewModelIsCreatedShouldNotNotifyCollectionChanged()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            Assert.AreEqual(0, _collectionChangedCount);
        }

        [TestMethod]
        public void WhenSortOrderIsChangedShouldNotifyViewModelCollectionChanged()
        {
            _viewModel.SortOrder = ContactListSortOrder.FirstName;
            ContactViewModel firstByFirstName = _viewModel.Contacts.First();

            _viewModel.SortOrder = ContactListSortOrder.LastName;
            Assert.AreEqual(1, _collectionChangedCount);
            Assert.AreEqual("Charles Babbage", _viewModel.Contacts.First().FullName);
        }

        [TestMethod]
        public void WhenSortOrderIsChangedButNotDifferentWeShouldNotGetACollectionChanged()
        {
            ContactViewModel defaultOrder = _viewModel.Contacts.First();
            _viewModel.SortOrder = ContactListSortOrder.FirstName;

            ContactViewModel firstByFirstName = _viewModel.Contacts.First();
            _viewModel.SortOrder = ContactListSortOrder.FirstName;

            Assert.AreEqual(1, _collectionChangedCount);

        }
    }
}
