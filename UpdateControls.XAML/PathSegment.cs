using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UpdateControls.XAML
{
	public class PathSegment
	{
		private string _identifier;
		private MethodInfo _getMethod;
		private MethodInfo _setMethod;

		public PathSegment(string identifier)
		{
			_identifier = identifier;
		}

		public Type CacheMethodInfo(Type contextType)
		{
			_getMethod = null;
			_setMethod = null;

			if (contextType != null)
			{
				// Get the method info of the identified property.
				MemberInfo[] member = contextType.GetMember(_identifier);
				if (member != null && member.Length == 1)
				{
					PropertyInfo propertyInfo = member[0] as PropertyInfo;
					if (propertyInfo != null && propertyInfo.CanRead)
					{
						_getMethod = propertyInfo.GetGetMethod();
						_setMethod = propertyInfo.GetSetMethod();
					}
					// Return the property type so that the chain can continue.
					return propertyInfo.PropertyType;
				}
			}
			return null;
		}

		public bool CanGet
		{
			get { return _getMethod != null; }
		}

		public bool CanSet
		{
			get { return _setMethod != null; }
		}

		public object Get(object context)
		{
			return _getMethod.Invoke(context, null);
		}

		public void Set(object context, object value)
		{
			_setMethod.Invoke(context, new object[] { value });
		}
	}
}
