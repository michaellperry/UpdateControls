using System;
using System.Collections.Generic;

namespace UpdateControls.XAML.Wrapper
{
    class AffectedSet
    {
        private static ThreadLocal<AffectedSet> _currentSet = new ThreadLocal<AffectedSet>();

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

        public static bool CaptureDependent(DependentProperty dependentProperty)
        {
            AffectedSet currentSet = _currentSet.Get();
            if (currentSet != null)
            {
                currentSet._dependentProperties.Add(dependentProperty);
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<DependentProperty> _dependentProperties = new List<DependentProperty>();

        public IEnumerable<DependentProperty> End()
        {
            System.Diagnostics.Debug.Assert(_currentSet.Get() == this);
            _currentSet.Set(null);
            return _dependentProperties;
        }
    }
}
