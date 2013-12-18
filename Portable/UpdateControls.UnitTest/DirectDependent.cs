using UpdateControls.Fields;

namespace UpdateControls.UnitTest
{
	public class DirectDependent
	{
		private SourceData _source;

        private Dependent<int> _property;

		public DirectDependent(SourceData source)
		{
			_source = source;
            _property = new Dependent<int>(() => _source.SourceProperty);
		}

		public int DependentProperty
		{
            get { return _property; }
		}

		public bool IsUpToDate
		{
			get { return _property.DependentSentry.IsUpToDate; }
		}
	}
}
