using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    public class BindingInterceptor
    {
        public static BindingInterceptor Current = new BindingInterceptor();

        public virtual object GetValue(ObjectProperty property) { return property.ContinueGetValue(); }
        public virtual void SetValue(ObjectProperty property, object value) { property.ContinueSetValue(value); }
        public virtual void UpdateValue(ObjectProperty property) { property.ContinueUpdateValue(); }
        public virtual void Execute(ObjectPropertyCommand command) { command.ContinueExecute(); }
    }
}
