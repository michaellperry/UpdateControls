using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdateControls.UnitTest
{
	[TestClass]
	public class IndirectConcurrencyTest
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
		public void DependentIsOutOfDateAfterConcurrentChange()
		{
			_source.AfterGet += () => _source.SourceProperty = 4;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;

			Assert.IsFalse(_dependent.IsUpToDate, "The dependent is up to date after a concurrent change");
		}

		[TestMethod]
		public void DependentHasOriginalValueAfterConcurrentChange()
		{
			_source.AfterGet += () => _source.SourceProperty = 4;

			_source.SourceProperty = 3;
			Assert.AreEqual(3, _dependent.DependentProperty);
		}

		[TestMethod]
		public void DependentIsUpToDateAfterSecondGet()
		{
			Action concurrentChange = () => _source.SourceProperty = 4;
			_source.AfterGet += concurrentChange;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;

			_source.AfterGet -= concurrentChange;

			fetch = _dependent.DependentProperty;

			Assert.IsTrue(_dependent.IsUpToDate, "The dependent is not up to date after the second get");
		}

		[TestMethod]
		public void DependentHasModifiedValueAfterSecondGet()
		{
			Action concurrentChange = () => _source.SourceProperty = 4;
			_source.AfterGet += concurrentChange;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;

			_source.AfterGet -= concurrentChange;

			Assert.AreEqual(4, _dependent.DependentProperty);
		}

		[TestMethod]
		public void DependentStillDependsUponPrecedent()
		{
			Action concurrentChange = () => _source.SourceProperty = 4;
			_source.AfterGet += concurrentChange;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;

			_source.AfterGet -= concurrentChange;

			fetch = _dependent.DependentProperty;
			_source.SourceProperty = 5;

			Assert.IsFalse(_dependent.IsUpToDate, "The dependent no longer depends upon the precedent");
		}

		[TestMethod]
		public void DependentGetsTheUltimateValue()
		{
			Action concurrentChange = () => _source.SourceProperty = 4;
			_source.AfterGet += concurrentChange;

			_source.SourceProperty = 3;
			int fetch = _dependent.DependentProperty;

			_source.AfterGet -= concurrentChange;

			fetch = _dependent.DependentProperty;
			_source.SourceProperty = 5;

			Assert.AreEqual(5, _dependent.DependentProperty);
		}
	}
}
