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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Markup;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassPropertyReal : ClassProperty
    {
        private PropertyInfo _propertyInfo;

        public ClassPropertyReal(PropertyInfo property, Type objectInstanceType)
            : base(property.Name, property.PropertyType, objectInstanceType)
        {
            _propertyInfo = property;
        }

        public override object GetObjectValue(object wrappedObject)
		{
			// Get the property from the wrapped object.
			return _propertyInfo.GetValue(wrappedObject, null);
		}

        public override void SetObjectValue(object wrappedObject, object value)
		{
            if (_propertyInfo.CanWrite)
    			_propertyInfo.SetValue(wrappedObject, value, null);
		}

		public override bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }

		public override Type UnderlyingType
		{
			get { return _propertyInfo.PropertyType; }
		}

        public override bool IsReadOnly
        {
            get { return !_propertyInfo.CanWrite; }
        }
	}
}
