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
							ObjectInstance.Tree.RemoveKey(oldValue);
							oldChild.Dispose();
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
                var scheduler = UpdateScheduler.Begin();

                try
                {
                    if (_hasChildObject && value != null)
                    {
                        value = ((IObjectInstance)value).WrappedObject;
                    }
                    ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);
                }
                finally
                {
                    if (scheduler != null)
                    {
                        using (NotificationGate.BeginOutbound())
                        {
                            foreach (IUpdatable updatable in scheduler.End())
                                updatable.UpdateNow();
                        }
                    }
                }
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
			{
				ObjectInstance.Tree.RemoveKey(_child.WrappedObject);
				_child.Dispose();
			}
            using (NotificationGate.BeginOutbound())
            {
                ObjectInstance.ClearValue(ClassProperty.DependencyProperty);
            }
            _depProperty.Dispose();
        }
	}
}
