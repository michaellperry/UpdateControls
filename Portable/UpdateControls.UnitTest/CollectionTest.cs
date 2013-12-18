using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.UnitTest.CollectionData;

namespace UpdateControls.UnitTest
{
    [TestClass]
    public class CollectionTest
    {
        private SourceCollection _source;

        [TestInitialize]
        public void Initialize()
        {
            _source = new SourceCollection();
        }

        [TestMethod]
        public void InsertIntoSourceShouldCauseNewElementInTarget()
        {
            TargetCollection target = new TargetCollection(_source);

            _source.Insert(3);
            int firstNumber = target.Results.Single();
            Assert.AreEqual(4, firstNumber);
        }

        [TestMethod]
        public void InsertIntoSourceBeforeTargetCreatedShouldCauseNewElementInTarget()
        {
            _source.Insert(3);

            TargetCollection target = new TargetCollection(_source);

            int firstNumber = target.Results.Single();
            Assert.AreEqual(4, firstNumber);
        }

        [TestMethod]
        public void InsertASecondIntoSourceTargetShouldUpdate()
        {
            TargetCollection target = new TargetCollection(_source);
            _source.Insert(42);
            target.Results.Single();

            _source.Insert(43);
            int[] results = target.Results.ToArray();

            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(43, results[0]);
            Assert.AreEqual(44, results[1]);
        }
    }
}
