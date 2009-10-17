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
	internal class ObjectPropertyCollectionNative : ObjectPropertyCollection
	{
        public ObjectPropertyCollectionNative(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
		}

        public override object TranslateOutgoingValue(object value)
        {
            return value;
        }
    }
}
