using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace UpdateControls.Wrapper
{
	internal abstract class ObjectPropertyCollection : ObjectProperty
	{
		private ObservableCollection<object> _collection = new ObservableCollection<object>();

		public ObjectPropertyCollection(DependencyObject dependencyObject, ClassProperty classProperty, object wrappedObject)
			: base(dependencyObject, classProperty, wrappedObject)
		{
			throw new NotImplementedException();
		}

		public override object TranslateIncommingValue(object value)
		{
			throw new NotSupportedException("Update Controls does not support two-way binding to collection properties.");
		}

		public override object TranslateOutgoingValue(object value)
		{
			return _collection;
		}
	}
}
