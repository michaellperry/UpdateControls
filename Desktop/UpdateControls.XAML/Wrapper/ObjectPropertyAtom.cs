/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using UpdateControls;

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;
        private object _value;
		private bool _firePropertyChanged = false;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
					object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
					object translatedValue = TranslateOutgoingValue(value);
					bool changed = !Object.Equals(_value, translatedValue);
					_value = translatedValue;
					if (changed && _firePropertyChanged)
						ObjectInstance.FirePropertyChanged(ClassProperty.Name);
					_firePropertyChanged = true;
				});
				// When the property becomes out of date, trigger an update.
				// The update should have lower priority than user input & drawing,
				// to ensure that the app doesn't lock up in case a large model is 
				// being updated outside the UI (e.g. via timers or the network).
				Action triggerUpdate = new Action(delegate
				{
					ObjectInstance.Dispatcher.BeginInvoke(new Action(delegate
					{
						using (NotificationGate.BeginOutbound())
						{
							_depProperty.OnGet();
						}
					}), System.Windows.Threading.DispatcherPriority.Background);
				});
				_depProperty.Invalidated += triggerUpdate;
			}
		}

		public override void OnUserInput(object value)
		{
            if (NotificationGate.IsInbound)
            {
                value = TranslateIncommingValue(value);
                ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);
            }
		}

        public override object Value
        {
            get
            {
                using (NotificationGate.BeginOutbound())
                {
                    if (_depProperty.IsNotUpdating)
                        _depProperty.OnGet();
                }
                return _value;
            }
        }

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
	}
}
