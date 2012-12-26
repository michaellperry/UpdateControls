using System;
using System.Collections.Generic;

namespace UpdateControls
{
    public class UpdateScheduler
    {
        private static Action<Action> _runOnUIThread;
        [ThreadStatic]
        private static UpdateScheduler _currentScheduler;

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
            if (_currentScheduler != null)
                return null;

            UpdateScheduler currentSet = new UpdateScheduler();
            _currentScheduler = currentSet;
            return currentSet;
        }

        public static void ScheduleUpdate(IUpdatable updatable)
        {
            UpdateScheduler currentSet = _currentScheduler;
            if (currentSet != null)
                currentSet._updatables.Add(updatable);
            else if (_runOnUIThread != null)
                _runOnUIThread(updatable.UpdateNow);
            else
                throw new InvalidOperationException(
                    "Please call ForView.Initialize() on the UI thread.");
        }

        private List<IUpdatable> _updatables = new List<IUpdatable>();

        public IEnumerable<IUpdatable> End()
        {
            System.Diagnostics.Debug.Assert(_currentScheduler == this);
            _currentScheduler = null;
            return _updatables;
        }
    }
}
