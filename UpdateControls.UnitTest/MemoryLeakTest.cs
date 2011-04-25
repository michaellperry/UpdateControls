using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Fields;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class MemoryLeakTest
    {
        [TestMethod]
        public void IndependentIsAsSmallAsPossible()
        {
            long start = GC.GetTotalMemory(true);
            Independent<int> newIndependent = new Independent<int>();
            newIndependent.Value = 42;
            long end = GC.GetTotalMemory(true);

            // Started at 92.
            // Making Precedent a base class: 80.
            // Removing Gain/LoseDependent events: 72.
            // Custom linked list implementation for dependents: 48.
            Assert.AreEqual(48, end - start);

            int value = newIndependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void DependentIsAsSmallAsPossible()
        {
            long start = GC.GetTotalMemory(true);
            Dependent<int> newDependent = new Dependent<int>(() => 42);
            long end = GC.GetTotalMemory(true);

            // Started at 260.
            // Making Precedent a base class: 248.
            // Removing Gain/LoseDependent events: 232.
            // Making IsUpToDate no longer a precident: 192.
            // Custom linked list implementation for dependents: 152.
            Assert.AreEqual(152, end - start);

            int value = newDependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void SingleDependencyBeforeUpdateIsAsSmallAsPossible()
        {
            long start = GC.GetTotalMemory(true);
            Independent<int> newIndependent = new Independent<int>();
            Dependent<int> newDependent = new Dependent<int>(() => newIndependent);
            newIndependent.Value = 42;
            long end = GC.GetTotalMemory(true);

            // Started at 336.
            // Making Precedent a base class: 312.
            // Removing Gain/LoseDependent events: 288.
            // Making IsUpToDate no longer a precident: 248.
            // Custom linked list implementation for dependents: 200.
            Assert.AreEqual(200, end - start);

            int value = newDependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void SingleDependencyAfterUpdateIsAsSmallAsPossible()
        {
            long start = GC.GetTotalMemory(true);
            Independent<int> newIndependent = new Independent<int>();
            Dependent<int> newDependent = new Dependent<int>(() => newIndependent);
            newIndependent.Value = 42;
            int value = newDependent;
            long end = GC.GetTotalMemory(true);

            // Started at 460.
            // Making Precedent a base class: 436.
            // Removing Gain/LoseDependent events: 412.
            // Making IsUpToDate no longer a precident: 372.
            // Custom linked list implementation for dependents: 308.
            Assert.AreEqual(308, end - start);

            value = newDependent;
            Assert.AreEqual(42, value);
        }
    }
}
