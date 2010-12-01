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

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;
        private IObjectInstance _child;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
                    _child = null;
					object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
                    value = TranslateOutgoingValue(value);
                    ClassProperty.SetUserOutput(ObjectInstance, value);
				});
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

        public override void UpdateNodes()
        {
            using (NotificationGate.BeginOutbound())
            {
                _depProperty.OnGet();
            }
            if (_child != null)
                _child.UpdateNodes();
        }

        protected void SetChild(IObjectInstance child)
        {
            _child = child;
        }

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
	}
}
