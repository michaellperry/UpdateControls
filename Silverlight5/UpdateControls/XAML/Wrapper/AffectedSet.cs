using System;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    public class AffectedSet
    {
        [ThreadStatic]
        private static AffectedSet _currentSet = null;

        public static AffectedSet Begin()
        {
            // If someone is already capturing the affected set,
            // let them keep that responsibility.
            if (_currentSet != null)
                return null;

            _currentSet = new AffectedSet();
            return _currentSet;
        }

        public static bool CaptureDependent(Dependent dependent)
        {
            if (_currentSet != null)
            {
                _currentSet._dependents.Add(dependent);
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Dependent> _dependents = new List<Dependent>();

        public IEnumerable<Dependent> End()
        {
            System.Diagnostics.Debug.Assert(_currentSet == this);
            _currentSet = null;
            return _dependents;
        }
    }
}
