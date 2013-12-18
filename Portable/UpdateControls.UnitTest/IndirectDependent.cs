using UpdateControls.Fields;

namespace UpdateControls.UnitTest
{
	public class IndirectDependent
	{
		private DirectDependent _indermediateDependent;

        private Dependent<int> _property;

        public IndirectDependent(DirectDependent indermediateDependent)
		{
			_indermediateDependent = indermediateDependent;
            _property = new Dependent<int>(() => _indermediateDependent.DependentProperty);
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
