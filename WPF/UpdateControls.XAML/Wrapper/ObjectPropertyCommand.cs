using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpdateControls.XAML.Wrapper
{
    public class ObjectPropertyCommand : ObjectProperty, ICommand
    {
        ObjectProperty _canProperty;

        public ClassMemberCommand ClassCommand { get; private set; }

        public ObjectProperty CanExecuteProperty
        {
            get
            {
                if (_canProperty == null && ClassCommand.CanExecuteProperty != null)
                {
                    _canProperty = ObjectInstance.LookupProperty(ClassCommand.CanExecuteProperty);
                    _canProperty.PropertyChanged += (sender, args) =>
                    {
                        if (CanExecuteChanged != null)
                            CanExecuteChanged(this, EventArgs.Empty);
                    };
                }
                return _canProperty;
            }
        }

        public event EventHandler CanExecuteChanged;

        public ObjectPropertyCommand(IObjectInstance objectInstance, ClassMemberCommand classCommand)
            : base(objectInstance, classCommand)
        {
            ClassCommand = classCommand;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteProperty != null)
                return (bool)CanExecuteProperty.Value;
            else
                return true;
        }

        public void Execute(object parameter)
        {
            BindingInterceptor.Current.Execute(this);
        }

        internal void ContinueExecute()
        {
            ClassCommand.ExecuteMethod.Invoke(ObjectInstance.WrappedObject, new object[0]);
        }

        protected override object GetValue() { return this; }
        protected override void SetValue(object value) { }
        protected override void UpdateValue() { }
    }
}
