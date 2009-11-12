
namespace UpdateControls.UnitTest
{
	public class IndirectDependent
	{
		private DirectDependent _indermediateDependent;

		private int _property;
		private Dependent _depProperty;

        public IndirectDependent(DirectDependent indermediateDependent)
		{
			_indermediateDependent = indermediateDependent;
			_depProperty = new Dependent(UpdateProperty);
		}

		public int DependentProperty
		{
			get { _depProperty.OnGet(); return _property; }
		}

		public bool IsUpToDate
		{
			get { return _depProperty.IsUpToDate; }
		}

		private void UpdateProperty()
		{
			_property = _indermediateDependent.DependentProperty;
		}
	}
}
