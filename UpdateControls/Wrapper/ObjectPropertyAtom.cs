using System;
using System.Collections;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using UpdateControls;

namespace UpdateControls.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty
    {
        private Dependent _depProperty;

        public ObjectPropertyAtom(ObjectInstance objectInstance, ClassProperty classProperty, object wrappedObject)
            : base(objectInstance, classProperty, wrappedObject)
        {
            if (classProperty.CanRead)
            {
                // When the property is out of date, update it from the wrapped object.
                _depProperty = new Dependent(delegate
                {
                    _classProperty.UpdateProperty(_objectInstance, _wrappedObject);
                });
                // When the property becomes out of date, trigger an update.
                Action triggerUpdate = new Action(delegate
                {
                    _objectInstance.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        _depProperty.OnGet();
                    }));
                });
                _depProperty.Invalidated += triggerUpdate;
                // The property is out of date right now, so trigger the first update.
                triggerUpdate();
            }
        }

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);
    }
}
