using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpdateControls.XAML.Wrapper
{
    public class ClassMemberCommand : ClassMember
    {
        public MethodInfo ExecuteMethod { get; private set; }
        public ClassMember CanExecuteProperty { get; private set; }

        public ClassMemberCommand(MethodInfo method, ClassMember condition, Type objectInstanceType)
            : base(method.Name, typeof(ObjectPropertyCommand), objectInstanceType)
        {
            ExecuteMethod = method;
            CanExecuteProperty = condition;
        }

        public override object GetObjectValue(object wrappedObject) { return null; }
        public override void SetObjectValue(object wrappedObject, object value) { }

        public override bool CanRead
        {
            get { return true; }
        }

        public override Type UnderlyingType
        {
            get { return typeof(ObjectPropertyCommand); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }
    }
}
