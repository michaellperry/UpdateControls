/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2009 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using UpdateControls;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;
        private object _value;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
					object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
					_value = TranslateOutgoingValue(value);
                    ObjectInstance.FirePropertyChanged(ClassProperty.Name);
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

        public override object Value
        {
            get { return _value; }
        }

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
	}
}
