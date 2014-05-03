using System;

namespace UpdateControls.Test
{
	public class PrefixViewModel
	{
		private PrefixID _prefix;

		public PrefixViewModel(PrefixID prefix)
		{
			_prefix = prefix;
		}

		public PrefixID Prefix
		{
			get { return _prefix; }
		}

		public override string ToString()
		{
			return _prefix.ToString();
		}
	}
}
