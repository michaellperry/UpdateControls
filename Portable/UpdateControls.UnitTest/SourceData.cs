using System;
using System.Threading;
using UpdateControls.Fields;

namespace UpdateControls.UnitTest
{
	public class SourceData
	{
		private Independent<int> _sourceProperty = new Independent<int>();
		private AutoResetEvent _continue = new AutoResetEvent(false);

		public int SourceProperty
		{
			get
			{
				int result = _sourceProperty;
				ThreadPool.QueueUserWorkItem(o =>
				{
					if (AfterGet != null)
						AfterGet();
					_continue.Set();
				});
				_continue.WaitOne();
				return result;
			}
			set { _sourceProperty.Value = value; }
		}

		public event Action AfterGet;
	}
}
