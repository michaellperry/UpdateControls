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

namespace UpdateControls.Fields
{
    public class Independent<T>
    {
        private T _value;
        private readonly Independent _independentSentry = new Independent();

        public Independent()
        {
        }

        public Independent(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get { _independentSentry.OnGet(); return _value; }
            set
            {
                if (!object.Equals(value, _value))
                {
                    _independentSentry.OnSet();
                    _value = value;
                }
            }
        }

        public Independent IndependentSentry
        {
            get { return _independentSentry; }
        }

        public static implicit operator T(Independent<T> independent)
        {
            return independent.Value;
        }
    }
}