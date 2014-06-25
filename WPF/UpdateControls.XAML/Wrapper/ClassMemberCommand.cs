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
        MethodInfo _method;
        ClassMember _condition;

        public ClassMemberCommand(MethodInfo method, ClassMember condition, Type objectInstanceType)
            : base(method.Name, typeof(ICommand), objectInstanceType)
        {
            _method = method;
            _condition = condition;
        }

        public override object GetObjectValue(object wrappedObject)
        {
            Action execute = () => BindingInterceptor.Current.Execute(wrappedObject, this);
            if (_condition != null)
                return MakeCommand.When(() => BindingInterceptor.Current.CanExecute(wrappedObject, this)).Do(execute);
            else
                return MakeCommand.Do(execute);
        }

        public override void SetObjectValue(object wrappedObject, object value)
        {
        }

        internal bool ContinueCanExecute(object wrappedObject)
        {
            return (bool)_condition.GetObjectValue(wrappedObject);
        }

        internal void ContinueExecute(object wrappedObject)
        {
            _method.Invoke(wrappedObject, new object[0]);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override Type UnderlyingType
        {
            get { return typeof(ICommand); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }
    }
}
