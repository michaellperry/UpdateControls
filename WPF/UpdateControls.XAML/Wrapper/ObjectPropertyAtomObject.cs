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
using System.Collections;

namespace UpdateControls.XAML.Wrapper
{
    class ObjectPropertyAtomObject : ObjectPropertyAtom
    {
        public ObjectPropertyAtomObject(IObjectInstance objectInstance, ClassMember classProperty)
            : base(objectInstance, classProperty)
        {
        }

        public override object TranslateIncommingValue(object value)
        {
            var instance = value as IObjectInstance;
            return instance != null ? instance.WrappedObject : value;
        }

        public override object TranslateOutgoingValue(object value)
        {
            if (value == null)
                return null;
            if (ClassProperty.UnderlyingType == typeof(object) && (ClassMember.IsPrimitive(value.GetType()) || typeof(IEnumerable).IsAssignableFrom(value.GetType())))
                return value;
            return WrapObject(value);
        }
    }
}
