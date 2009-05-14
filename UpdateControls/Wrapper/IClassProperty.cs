using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
	interface IClassProperty
	{
		bool CanRead { get; }
        Type PropertyType { get; }
        void UpdateProperty(DependencyObject obj, object wrappedObject);
	}
}
