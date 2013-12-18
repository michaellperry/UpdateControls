using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdateControls.UnitTest
{
    public class NotifyingIndependent : Independent
    {
        public event Action OnGainDependent;
        public event Action OnLoseDependent;

        protected override void GainDependent()
        {
            if (OnGainDependent != null)
                OnGainDependent();
        }

        protected override void LoseDependent()
        {
            if (OnLoseDependent != null)
                OnLoseDependent();
        }
    }

    [TestClass]
    public class NotificationTest
    {
        private bool _gained;
        private bool _lost;
        private NotifyingIndependent _independent;
        private Dependent _dependent;
        private Dependent _secondDependent;

        [TestInitialize]
        public void Initialize()
        {
            _gained = false;
            _independent = new NotifyingIndependent();
            _independent.OnGainDependent += () => { _gained = true; };
            _independent.OnLoseDependent += () => { _lost = true; };
            _dependent = new Dependent(() => { _independent.OnGet(); });
            _secondDependent = new Dependent(() => { _independent.OnGet(); });
        }

        [TestMethod]
        public void DoesNotGainDependentOnCreation()
        {
            Assert.IsFalse(_gained, "The independent should not have gained a dependent.");
        }

        [TestMethod]
        public void GainsDependentOnFirstUse()
        {
            _dependent.OnGet();
            Assert.IsTrue(_gained, "The independent should have gained a dependent.");
        }

        [TestMethod]
        public void DoesNotGainDependentOnSecondUse()
        {
            _dependent.OnGet();
            _gained = false;
            _secondDependent.OnGet();
            Assert.IsFalse(_gained, "The independent should not have gained a dependent.");
        }

        [TestMethod]
        public void DoesNotLoseDependentOnCreation()
        {
            Assert.IsFalse(_lost, "The independent should not have lost a dependent.");
        }

        [TestMethod]
        public void DoesNotLoseDependentOnFirstUse()
        {
            _dependent.OnGet();
            Assert.IsFalse(_lost, "The independent should not have lost a dependent.");
        }

        [TestMethod]
        public void LosesDependentWhenChanging()
        {
            _dependent.OnGet();
            _independent.OnSet();
            Assert.IsTrue(_lost, "The independent should have lost a dependent.");
        }
    }
}
