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

namespace UpdateControls.XAML.Wrapper
{
	class ObjectPropertyAtomNative : ObjectPropertyAtom
    {

        public ObjectPropertyAtomNative(IObjectInstance objectInstance, ClassProperty classProperty)
			: base(objectInstance, classProperty)
		{
        }
        public override object TranslateIncommingValue(object value)
		{
			return value;
		}

		public override object TranslateOutgoingValue(object value)
		{
			return value;
		}
	}
}
