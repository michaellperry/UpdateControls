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
using System.ComponentModel;
using System.Reflection;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassEvent : EventDescriptor
    {
        private EventInfo _eventInfo;

        public ClassEvent(EventInfo eventInfo)
            : base(eventInfo.Name, null)
        {
            _eventInfo = eventInfo;
        }

        public override void AddEventHandler(object component, Delegate value)
        {
            // Add the event handler to the wrapped object.
            _eventInfo.AddEventHandler(GetTarget(component), value);
        }

        public override void RemoveEventHandler(object component, Delegate value)
        {
            // Remove the event handler from the wrapped object.
            _eventInfo.RemoveEventHandler(GetTarget(component), value);
        }

        public override Type ComponentType
        {
            get { return _eventInfo.DeclaringType; }
        }

        public override Type EventType
        {
            get { return _eventInfo.EventHandlerType; }
        }

        public override bool IsMulticast
        {
            get { return _eventInfo.IsMulticast; }
        }

        private static object GetTarget(object component)
        {
            return ((IObjectInstance)component).WrappedObject;
        }
    }
}
