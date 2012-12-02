using System;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    public class AffectedSet
    {
        private static Action<Action> _runOnUIThread;
        private static ThreadLocal<AffectedSet> _currentSet = new ThreadLocal<AffectedSet>();

        public static void Initialize(Action<Action> runOnUIThread)
        {
            if (_runOnUIThread == null)
            {
                _runOnUIThread = runOnUIThread;
            }
        }

        public static AffectedSet Begin()
        {
            // If someone is already capturing the affected set,
            // let them keep that responsibility.
            if (_currentSet.Get() != null)
                return null;

            AffectedSet currentSet = new AffectedSet();
            _currentSet.Set(currentSet);
            return currentSet;
        }

        public static void CaptureDependent(IUpdatable updatable)
        {
            AffectedSet currentSet = _currentSet.Get();
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
            System.Diagnostics.Debug.Assert(_currentSet.Get() == this);
            _currentSet.Set(null);
            return _updatables;
        }
    }
}
