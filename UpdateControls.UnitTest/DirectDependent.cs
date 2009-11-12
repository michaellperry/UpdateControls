using System;

namespace UpdateControls.UnitTest
{
	public class DirectDependent
	{
		private SourceData _source;

		private int _property;
		private Dependent _depProperty;

		public DirectDependent(SourceData source)
		{
			_source = source;
			_depProperty = new Dependent(UpdateDependent);
		}

		public int DependentProperty
		{
			get
			{
				_depProperty.OnGet();
				return _property;
			}
		}

		private void UpdateDependent()
		{
			_property = _source.SourceProperty;
		}

		public bool IsUpToDate
		{
			get { return _depProperty.IsUpToDate; }
		}
	}
}
