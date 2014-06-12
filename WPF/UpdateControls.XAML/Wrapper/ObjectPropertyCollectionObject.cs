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
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace UpdateControls.XAML.Wrapper
{
    internal class ObjectPropertyCollectionObject : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionObject(IObjectInstance objectInstance, ClassMember classProperty)
			: base(objectInstance, classProperty)
		{
		}

        public override object TranslateOutgoingValue(object value)
        {
            return value == null ? null : WrapObject(value);
        }
		public override object TranslateIncomingValue(object value)
		{
			return ((IObjectInstance)value).WrappedObject;
		}
    }
}
