using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassField : ClassProperty
    {
        private FieldInfo _fieldInfo;

        public ClassField(FieldInfo field, Type objectInstanceType)
            : base(field.Name, field.FieldType, objectInstanceType)
        {
            _fieldInfo = field;
        }

		public override object GetObjectValue(object wrappedObject)
		{
            return _fieldInfo.GetValue(wrappedObject);
		}

        public override void SetObjectValue(object wrappedObject, object value)
		{
            _fieldInfo.SetValue(wrappedObject, value);
		}

		public override bool CanRead
        {
            get { return true; }
        }

		public override Type UnderlyingType
		{
            get { return _fieldInfo.FieldType; }
		}

        public override bool IsReadOnly
        {
            get { return _fieldInfo.IsInitOnly; }
        }
    }
}
