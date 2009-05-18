/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System;
using UpdateControls;

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;

		public ObjectPropertyAtom(ObjectInstance objectInstance, ClassProperty classProperty)
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
					ObjectInstance.Dispatcher.BeginInvoke(new Action(delegate
					{
						_depProperty.OnGet();
					}));
				});
				_depProperty.Invalidated += triggerUpdate;
				// The property is out of date right now, so trigger the first update.
				triggerUpdate();
			}
		}

		public override void OnUserInput(object value)
		{
            if (_depProperty.IsNotUpdating)
            {
                value = TranslateIncommingValue(value);
                ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);
            }
		}

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
	}
}
