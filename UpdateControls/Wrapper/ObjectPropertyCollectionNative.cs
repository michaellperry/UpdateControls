using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;

namespace UpdateControls.Wrapper
{
	internal class ObjectPropertyCollectionNative : ObjectPropertyCollection
	{
		public ObjectPropertyCollectionNative(DependencyObject dependencyObject, ClassProperty classProperty, object wrappedObject)
			: base(dependencyObject, classProperty, wrappedObject)
		{
		}
	}
}
