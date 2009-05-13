using System.Reflection;
using System.Windows;
using System;

namespace UpdateControls.Wrapper
{
	interface IClassProperty
	{
		bool CanRead { get; }
		void UpdateProperty(DependencyObject obj, object wrappedObject);
	}
}
