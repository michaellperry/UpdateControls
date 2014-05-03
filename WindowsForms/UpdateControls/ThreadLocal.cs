using System.Collections.Generic;
using System.Threading;

namespace UpdateControls
{
    public class ThreadLocal<T>
    {
        private Dictionary<int, T> _valueByThread = new Dictionary<int, T>();

        public T Get()
        {
            lock (this)
            {
                T value;
                if (!_valueByThread.TryGetValue(Thread.CurrentThread.ManagedThreadId, out value))
                    return default(T);
                return value;
            }
        }

        public void Set(T value)
        {
            lock (this)
            {
                _valueByThread[Thread.CurrentThread.ManagedThreadId] = value;
            }
        }
    }
}
