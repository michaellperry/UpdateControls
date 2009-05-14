using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace UpdateControls.Wrapper
{
    internal class ObjectPropertyCollectionObject : ObjectPropertyCollection
	{
		public ObjectPropertyCollectionObject(DependencyObject dependencyObject, ClassProperty classProperty, object wrappedObject)
			: base(dependencyObject, classProperty, wrappedObject)
		{
		}
	}
}
