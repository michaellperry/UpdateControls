using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdateControls.UnitTest
{
    [TestClass]
	public class IndirectDependentTest
	{
		public TestContext TestContext { get; set; }

		private SourceData _source;
		private DirectDependent _intermediateDependent;
		private IndirectDependent _dependent;

		[TestInitialize]
		public void Initialize()
		{
			_source = new SourceData();
			_intermediateDependent = new DirectDependent(_source);
			_dependent = new IndirectDependent(_intermediateDependent);
		}

		[TestMethod]
		public void DependentIsInitiallyOutOfDate()
		{
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent is initially up to date");
		}

		[TestMethod]
		public void DependentRemainsOutOfDateOnChange()
		{
			_source.SourceProperty = 3;
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent is up to date after change");
		}

		[TestMethod]
		public void DependentIsUpdatedOnGet()
		{
			int fetch = _dependent.DependentProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent has not been updated");
		}

		[TestMethod]
		public void DependentIsUpdatedAfterChangeOnGet()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent has not been updated");
		}

		[TestMethod]
		public void DependentGetsValueFromItsPrecedent()
		{
			_source.SourceProperty = 3;
			Assert.AreEqual(3, _dependent.DependentProperty);
		}

		[TestMethod]
		public void DependentIsOutOfDateAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			_source.SourceProperty = 4;
			Assert.IsFalse(_dependent.IsUpToDate, "The dependent did not go out of date");
		}

		[TestMethod]
		public void DependentIsUpdatedAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			_source.SourceProperty = 4;
			fetch = _dependent.DependentProperty;
			Assert.IsTrue(_dependent.IsUpToDate, "The dependent did not get udpated");
		}

		[TestMethod]
		public void DependentGetsValueFromItsPrecedentAgainAfterChange()
		{
			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			_source.SourceProperty = 4;
			Assert.AreEqual(4, _dependent.DependentProperty);
		}

		[TestMethod]
		public void PrecedentIsOnlyAskedOnce()
		{
			int getCount = 0;
			_source.AfterGet += () => ++getCount;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			fetch = _dependent.DependentProperty;

			Assert.AreEqual(1, getCount);
		}

		[TestMethod]
		public void PrecedentIsAskedAgainAfterChange()
		{
			int getCount = 0;
			_source.AfterGet += () => ++getCount;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;
			fetch = _dependent.DependentProperty;
			_source.SourceProperty = 4;
			fetch = _dependent.DependentProperty;
			fetch = _dependent.DependentProperty;

			Assert.AreEqual(2, getCount);
		}
	}
}
