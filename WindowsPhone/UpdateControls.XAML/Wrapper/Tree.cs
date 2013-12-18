using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    public class Tree : IUpdatable
    {
        private Dictionary<object, IObjectInstance> _wrapperByObject = new Dictionary<object, IObjectInstance>();
        private IObjectInstance _root;
        private Dependent _depNodes;

        public Tree()
        {
            _depNodes = new Dependent(delegate
            {
                _root.UpdateNodes();
            });
            _depNodes.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
        }

        public void SetRoot(IObjectInstance root)
        {
            _root = root;
            _depNodes.Touch();
        }

        public bool WrapObject(object value, out IObjectInstance wrapper)
        {
            if (!_wrapperByObject.TryGetValue(value, out wrapper))
            {
                wrapper = (IObjectInstance)typeof(ObjectInstance<>)
                    .MakeGenericType(value.GetType())
                    .GetConstructors()
                    .Single()
                    .Invoke(new object[] { value, this });
                _wrapperByObject.Add(value, wrapper);
                return true;
            }
            else
                return false;
        }

        public void RemoveKey(object key)
        {
            _wrapperByObject.Remove(key);
        }

        public void UpdateNow()
        {
            _depNodes.OnGet();
        }
    }
}
