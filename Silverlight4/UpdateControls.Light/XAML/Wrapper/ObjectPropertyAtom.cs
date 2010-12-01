/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using UpdateControls;
using System.ComponentModel.DataAnnotations;

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
					object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
					value = TranslateOutgoingValue(value);
					ClassProperty.SetUserOutput(ObjectInstance, value);
				});
				// When the property becomes out of date, trigger an update.
				Action triggerUpdate = new Action(delegate
				{
					ObjectInstance.Defer(new Action(delegate
					{
                        using (NotificationGate.BeginOutbound())
                        {
                            _depProperty.OnGet();
                        }
					}));
				});
				_depProperty.Invalidated += triggerUpdate;
                // The property is out of date right now, so trigger the first update.
                _depProperty.Touch();
            }
		}

		public override void OnUserInput(object value)
		{
            if (NotificationGate.IsInbound)
            {
                value = TranslateIncommingValue(value);
				Validator.ValidateProperty(value, new ValidationContext(ObjectInstance.WrappedObject, null, null) { MemberName = ClassProperty.Name });
                ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);
            }
		}

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
	}
}
