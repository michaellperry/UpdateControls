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

namespace UpdateControls.XAML.Wrapper
{
    class ObjectPropertyAtomObject : ObjectPropertyAtom
    {
        public ObjectPropertyAtomObject(IObjectInstance objectInstance, ClassProperty classProperty)
            : base(objectInstance, classProperty)
        {
        }

        public override object TranslateIncommingValue(object value)
        {
            return value == null ? null : ((IObjectInstance)value).WrappedObject;
        }

        public override object TranslateOutgoingValue(object value)
        {
            return value == null ? null : WrapObject(value);
        }
    }
}
