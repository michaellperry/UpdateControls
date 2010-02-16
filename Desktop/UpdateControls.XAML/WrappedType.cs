using System;
using System.ComponentModel;
using System.Windows.Markup;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
	[ContentProperty("Type")]
	[MarkupExtensionReturnType(typeof(Type))]
	public class WrappedType : MarkupExtension
	{
		private Type _type;

		public WrappedType()
		{
		}

		public WrappedType(Type type)
		{
			_type = type;
		}

		[DefaultValue(null)]
		public Type Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return typeof(ObjectInstance<>).MakeGenericType(_type);
		}
	}
}