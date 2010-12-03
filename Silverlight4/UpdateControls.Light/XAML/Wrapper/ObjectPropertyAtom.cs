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

using System.ComponentModel.DataAnnotations;

namespace UpdateControls.XAML.Wrapper
{
    internal class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;
        private IObjectInstance _child;
        private bool _hasChildObject;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassProperty classProperty, bool hasChildObject)
			: base(objectInstance, classProperty)
		{
            _hasChildObject = hasChildObject;

			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
				_depProperty = new Dependent(delegate
				{
                    object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
                    if (_hasChildObject)
                    {
                        IObjectInstance oldChild = _child;
                        object oldValue = oldChild == null ? null : oldChild.WrappedObject;

                        _child = null;
                        IObjectInstance wrapper;
                        if (value == null)
                            wrapper = null;
                        else if (value == oldValue)
                        {
                            wrapper = oldChild;
                            _child = wrapper;
                        }
                        else
                        {
                            if (WrapObject(value, out wrapper))
                                _child = wrapper;
                        }
                        ClassProperty.SetUserOutput(ObjectInstance, wrapper);

                        if (oldChild != _child && oldChild != null)
                        {
                            oldChild.Dispose();
                            ObjectInstance.Tree.RemoveKey(oldValue);
                        }
                    }
                    else
                    {
                        ClassProperty.SetUserOutput(ObjectInstance, value);
                    }
				});
            }
		}

		public override void OnUserInput(object value)
		{
            if (NotificationGate.IsInbound)
            {
                if (_hasChildObject)
                {
                    value = value == null ? null : ((IObjectInstance)value).WrappedObject;
                }
				Validator.ValidateProperty(value, new ValidationContext(ObjectInstance.WrappedObject, null, null) { MemberName = ClassProperty.Name });
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

        public override void Dispose()
        {
            if (_child != null)
                _child.Dispose();
            ObjectInstance.ClearValue(ClassProperty.DependencyProperty);
            _depProperty.Dispose();
        }
	}
}
