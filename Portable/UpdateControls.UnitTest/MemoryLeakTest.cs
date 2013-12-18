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
#if SILVERLIGHT
        // In Silverlight, Dependent is 12 bytes bigger. Why?
        private const int DependentPlatformOffset = 12;
#else
        private const int DependentPlatformOffset = 0;
#endif
#if SILVERLIGHT
        // In Silverlight, Independent is 8 bytes smaller. Why?
        private static long IndependentPlatformOffset = -8;
#else
        private static long IndependentPlatformOffset = 0;
#endif

        [TestMethod]
        //[Ignore]
        public void IndependentIsAsSmallAsPossible()
        {
            GC.Collect();
            long start = GC.GetTotalMemory(true);
            Independent<int> newIndependent = new Independent<int>();
            newIndependent.Value = 42;
            long end = GC.GetTotalMemory(true);

            // Started at 92.
            // Making Precedent a base class: 80.
            // Removing Gain/LoseDependent events: 72.
            // Custom linked list implementation for dependents: 48.
            // Other optimizations: 40.
            // Removed WeakReferenceToSelf: 20.
            Assert.AreEqual(20 + IndependentPlatformOffset, end - start);

            int value = newIndependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        //[Ignore]
        public void DependentIsAsSmallAsPossible()
        {
            GC.Collect();
            long start = GC.GetTotalMemory(true);
            Dependent<int> newDependent = new Dependent<int>(() => 42);
            long end = GC.GetTotalMemory(true);

            // Started at 260.
            // Making Precedent a base class: 248.
            // Removing Gain/LoseDependent events: 232.
            // Making IsUpToDate no longer a precident: 192.
            // Custom linked list implementation for dependents: 152.
            // Custom linked list implementation for precedents: 112.
            // Other optimizations: 104.
			// Added WeakReferenceToSelf: 108.
            // Removed WeakReferenceToSelf: 104.
            Assert.AreEqual(104 + DependentPlatformOffset, end - start);

            int value = newDependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        //[Ignore]
        public void SingleDependencyBeforeUpdateIsAsSmallAsPossible()
        {
            GC.Collect();
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
            // Custom linked list implementation for precedents: 160.
            // Other optimizations: 144.
			// Added WeakReferenceToSelf: 148.
            // Removed WeakReferenceToSelf: 124.
			Assert.AreEqual(124 + IndependentPlatformOffset, end - start);

            int value = newDependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        [Ignore]
        public void SingleDependencyAfterUpdateIsAsSmallAsPossible()
        {
            GC.Collect();
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
            // Custom linked list implementation for precedents: 192.
            // Weak reference to dependents: 208.
            // Other optimizations: 192.
			// Added WeakReferenceToSelf: 196.
            // Removed WeakReferenceToSelf: 168 - 324.
			Assert.AreEqual(324 + IndependentPlatformOffset, end - start);

            value = newDependent;
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void DirectDependentObjectCanBeGarbageCollected()
        {
            GC.Collect();
            SourceData independent = new SourceData();
            DirectDependent dependent = new DirectDependent(independent);
            independent.SourceProperty = 42;
            Assert.AreEqual(42, dependent.DependentProperty);
            WeakReference weakDependent = new WeakReference(dependent);

            GC.Collect();
            Assert.IsTrue(weakDependent.IsAlive, "Since we hold a strong reference to the dependent, the object should still be alive.");
            // This assertion here to make sure the dependent is not optimized away.
            Assert.AreEqual(42, dependent.DependentProperty);

            dependent = null;
            GC.Collect();
            Assert.IsFalse(weakDependent.IsAlive, "Since we released the strong reference to the dependent, the object should not be alive.");

            // Make sure we can still modify the independent.
            independent.SourceProperty = 32;
            Assert.AreEqual(32, independent.SourceProperty);
        }

        [TestMethod]
        public void IndirectDependentObjectCanBeGarbageCollected()
        {
            GC.Collect();
            SourceData independent = new SourceData();
            DirectDependent intermediate = new DirectDependent(independent);
            IndirectDependent indirectDependent = new IndirectDependent(intermediate);
            independent.SourceProperty = 42;
            Assert.AreEqual(42, indirectDependent.DependentProperty);
            WeakReference weakIndirectDependent = new WeakReference(indirectDependent);

            GC.Collect();
            Assert.IsTrue(weakIndirectDependent.IsAlive, "Since we hold a strong reference to the dependent, the object should still be alive.");
            // This assertion here to make sure the dependent is not optimized away.
            Assert.AreEqual(42, indirectDependent.DependentProperty);

            indirectDependent = null;
            GC.Collect();
            Assert.IsFalse(weakIndirectDependent.IsAlive, "Since we released the strong reference to the dependent, the object should not be alive.");

            // Make sure we can still modify the independent, and that the intermediate still depends upon it.
            independent.SourceProperty = 32;
            Assert.AreEqual(32, independent.SourceProperty);
            Assert.AreEqual(32, intermediate.DependentProperty);
        }
    }
}
