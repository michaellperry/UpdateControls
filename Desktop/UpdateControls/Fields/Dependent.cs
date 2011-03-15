/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2011 Michael L Perry
 * MIT License
 * 
 * This class based on a contribution by David Piepgrass.
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;

namespace UpdateControls.Fields
{
    public class Dependent<T>
    {
        private Func<T> _computeValue;

        private T _value;
        private readonly Dependent _dependentSentry;

        public Dependent(Func<T> computeValue)
        {
            _computeValue = computeValue;
            _dependentSentry = new Dependent(() => _value = _computeValue());
        }

        public T Value
        {
            get { _dependentSentry.OnGet(); return _value; }
        }

        public Dependent DependentSentry
        {
            get { return _dependentSentry; }
        }

        public static implicit operator T(Dependent<T> dependent)
        {
            return dependent.Value;
        }
    }
}
