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

namespace UpdateControls.XAML.Wrapper
{
	class ObjectPropertyAtomNative : ObjectPropertyAtom
    {

        public ObjectPropertyAtomNative(IObjectInstance objectInstance, ClassMember classProperty)
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
