using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Collections;
using UpdateControls.UnitTest.ContactListData;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class DependentListTest
    {
        private ContactList _model;
        private DependentList<ContactViewModel> _contactViewModels;

        [TestInitialize]
        public void Initialize()
        {
            _model = new ContactList();
            _model.AddContact(new Contact() { FirstName = "Charles", LastName = "Babbage" });
            _model.AddContact(new Contact() { FirstName = "Alan", LastName = "Turing" });
            _contactViewModels = new DependentList<ContactViewModel>(() =>
                from c in _model.Contacts
                select new ContactViewModel(c)
            );
        }

        [TestMethod]
        public void DependentListMapsToSourceList()
        {
            Assert.AreEqual(2, _contactViewModels.Count);
            Assert.AreEqual("Charles Babbage", _contactViewModels[0].FullName);
            Assert.AreEqual("Alan Turing", _contactViewModels[1].FullName);
        }

        [TestMethod]
        public void WhenSourceListChanges_DependentListChanges()
        {
            _model.AddContact(new Contact() { FirstName = "Bertrand", LastName = "Meyer" });

            Assert.AreEqual(3, _contactViewModels.Count);
            Assert.AreEqual("Bertrand Meyer", _contactViewModels[2].FullName);
        }

        [TestMethod]
        public void DependentsAreRecycled()
        {
            ContactViewModel oldObject = _contactViewModels[0];
            _model.AddContact(new Contact() { FirstName = "Bertrand", LastName = "Meyer" });

            Assert.AreSame(oldObject, _contactViewModels[0]);
        }

        [TestMethod]
        public void ListDependsUponSourceCollection()
        {
            int triggerUpdate = _contactViewModels.Count;
            bool wasInvalidated = false;
            _contactViewModels.DependentSentry.Invalidated += delegate
            {
                wasInvalidated = true;
            };

            _model.AddContact(new Contact() { FirstName = "Bertrand", LastName = "Meyer" });

            Assert.IsTrue(wasInvalidated, "The dependent list should be invalidated.");
        }

        [TestMethod]
        public void ListDoesNotDependUponChildProperties()
        {
            int triggerUpdate = _contactViewModels.Count;
            bool wasInvalidated = false;
            _contactViewModels.DependentSentry.Invalidated += delegate
            {
                wasInvalidated = true;
            };

            _model.Contacts.First().FirstName = "George";

            Assert.IsFalse(wasInvalidated, "The dependent list should not be invalidated.");
        }
    }
}
