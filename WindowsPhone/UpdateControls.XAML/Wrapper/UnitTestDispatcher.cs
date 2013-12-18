using System;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    public static class UnitTestDispatcher
    {
        [ThreadStatic]
        private static List<Action> _deferredActions = new List<Action>();

        [ThreadStatic]
        public static bool On = false;

        public static void Defer(Action action)
        {
            _deferredActions.Add(action);
        }

        public static bool ExecuteAll()
        {
            int loopCount = 3;
            while (_deferredActions.Count > 0 && loopCount > 0)
            {
                List<Action> actions = new List<Action>(_deferredActions);
                _deferredActions.Clear();
                foreach (Action action in actions)
                    action();
                --loopCount;
            }
            return loopCount > 0;
        }
    }
}
