using System;
using System.Collections.Generic;
using System.Threading;

namespace UpdateControls
{
    public class UpdateScheduler
    {
        private static Action<Action> _runOnUIThread;
        private static ThreadLocal<UpdateScheduler> _currentSet = new ThreadLocal<UpdateScheduler>();

        public static void Initialize(Action<Action> runOnUIThread)
        {
            if (_runOnUIThread == null)
            {
                _runOnUIThread = runOnUIThread;
            }
        }

        public static UpdateScheduler Begin()
        {
            // If someone is already capturing the affected set,
            // let them keep that responsibility.
            if (_currentSet.Value != null)
                return null;

            UpdateScheduler currentSet = new UpdateScheduler();
            _currentSet.Value = currentSet;
            return currentSet;
        }

        public static void ScheduleUpdate(IUpdatable updatable)
        {
            UpdateScheduler currentSet = _currentSet.Value;
            if (currentSet != null)
                currentSet._updatables.Add(updatable);
            else if (_runOnUIThread != null)
                _runOnUIThread(updatable.UpdateNow);
        }

        private List<IUpdatable> _updatables = new List<IUpdatable>();

        public IEnumerable<IUpdatable> End()
        {
            System.Diagnostics.Debug.Assert(_currentSet.Value == this);
            _currentSet.Value = null;
            return _updatables;
        }
    }
}
