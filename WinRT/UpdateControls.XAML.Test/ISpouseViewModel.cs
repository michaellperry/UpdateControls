using System;

namespace UpdateControls.XAML.Test
{
	public interface ISpouseViewModel
	{
		Person Spouse { get; }
		string FullName { get; }
		bool Equals(object obj);
		int GetHashCode();
	}
}
