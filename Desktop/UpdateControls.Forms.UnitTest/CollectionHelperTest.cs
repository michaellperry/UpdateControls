using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Forms.Util;
using System.Collections;

namespace UpdateControls.Forms.UnitTest
{
    [TestClass]
    public class CollectionHelperTest
    {
        [TestMethod]
        public void SourceAndTargetAreEmpty()
        {
            var target = new NonClearableArrayList();
            CollectionHelper.RecycleCollection(target, Enumerable.Empty<int>());

            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(0, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void OnlySourceIsEmpty()
        {
            var target = new NonClearableArrayList();
            target.Add(1);
            target.Add(7);
            target.Add(42);
            CollectionHelper.RecycleCollection(target, Enumerable.Empty<int>());

            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(3, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void OnlyTargetIsEmpty()
        {
            var target = new NonClearableArrayList();
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 17));

            Assert.AreEqual(17, target.Count);
            Assert.AreEqual(0, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void SourceAndTargetDontOverlap()
        {
            var target = new NonClearableArrayList();
            target.Add(1);
            target.Add(2);
            target.Add(3);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(4, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(4, target[0]);
            Assert.AreEqual(5, target[1]);
            Assert.AreEqual(6, target[2]);
            Assert.AreEqual(3, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void SourceAndTargetOverlap()
        {
            var target = new NonClearableArrayList();
            target.Add(1);
            target.Add(2);
            target.Add(3);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(3, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(3, target[0]);
            Assert.AreEqual(4, target[1]);
            Assert.AreEqual(5, target[2]);
            Assert.AreEqual(2, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void InsertBeforeExistingItems()
        {
            var target = new NonClearableArrayList();
            target.Add(2);
            target.Add(3);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
            Assert.AreEqual(0, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void InsertBetweenExistingItems()
        {
            var target = new NonClearableArrayList();
            target.Add(1);
            target.Add(3);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
            Assert.AreEqual(0, target.ItemsRemovedCount);
        }

        [TestMethod]
        public void InsertAfterExistingItems()
        {
            var target = new NonClearableArrayList();
            target.Add(1);
            target.Add(2);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
            Assert.AreEqual(0, target.ItemsRemovedCount);
        }
 
        [TestMethod]
        public void SlideItemUp()
        {
            var target = new NonClearableArrayList();
            target.Add(2);
            target.Add(3);
            target.Add(1);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
            Assert.AreEqual(1, target.ItemsRemovedCount);
        }
 
        [TestMethod]
        public void SlideItemDown()
        {
            var target = new NonClearableArrayList();
            target.Add(3);
            target.Add(1);
            target.Add(2);
            CollectionHelper.RecycleCollection(target, Enumerable.Range(1, 3));

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);

            // This operation could be done in 1 removal, but the algorithm doesn't know that.
            Assert.AreEqual(2, target.ItemsRemovedCount);
        }
   }
}
