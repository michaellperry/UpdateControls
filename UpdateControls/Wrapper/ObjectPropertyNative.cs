using System;
using System.Windows;

namespace UpdateControls.Wrapper
{
	class ObjectPropertyNative : ObjectProperty
	{
		private Dependent _depProperty;

		public ObjectPropertyNative(DependencyObject dependencyObject, IClassProperty classProperty, object wrappedObject)
			: base(dependencyObject, classProperty, wrappedObject)
		{
			if (classProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
					_classProperty.UpdateProperty(_dependencyObject, _wrappedObject);
				});

				// When the property becomes out of date, trigger an update.
				Action triggerUpdate = new Action(
					delegate()
					{
						_dependencyObject.Dispatcher.BeginInvoke(new Action(
							delegate()
							{
								_depProperty.OnGet();
							}
						));
					}
				);
				_depProperty.Invalidated += triggerUpdate;

				// The property is out of date right now, so trigger the first update.
				triggerUpdate();
			}
		}

		public override object TranslateIncommingValue(object value)
		{
			return value;
		}

		public override object TranslateOutgoingValue(object value)
		{
			return value;
		}
	}
}
