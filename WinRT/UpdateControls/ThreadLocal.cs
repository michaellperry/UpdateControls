using System;
using System.Collections.Generic;
using System.Threading;

namespace UpdateControls
{
    public class ThreadLocal<T>
    {
        [ThreadStatic]
        private T _value;

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            _value = value;
        }
    }
}
