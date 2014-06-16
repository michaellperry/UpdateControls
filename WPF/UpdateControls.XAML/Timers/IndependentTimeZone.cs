using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.Timers
{
    public abstract class IndependentTimeZone
    {
        Dictionary<DateTime, WeakReference> _timers = new Dictionary<DateTime, WeakReference>();
        int _purgePressure;
        SortedSet<IndependentTimer> _queue = new SortedSet<IndependentTimer>(
            Comparer<IndependentTimer>.Create((l, r) => Comparer<DateTime>.Default.Compare(l.ExpirationTime, r.ExpirationTime)));

        public static readonly IndependentTimeZone Utc = new UtcTimeZone();

        public abstract DateTime GetRawTime();
        protected abstract void ScheduleTimer(DateTime time);
        protected abstract void CancelTimer();

        protected void NotifyTimerExpired()
        {
            ScheduleNext();
        }

        internal IndependentTimer GetTimer(DateTime time)
        {
            if (_timers.ContainsKey(time))
            {
                var cached = _timers[time].Target as IndependentTimer;
                if (cached != null)
                    return cached;
            }
            if (2 * _purgePressure >= _timers.Count)
            {
                foreach (var key in _timers.Keys.ToList())
                    if (!_timers[key].IsAlive)
                        _timers.Remove(key);
                _purgePressure = 0;
            }
            var created = new IndependentTimer(this, time);
            _timers[time] = new WeakReference(created);
            ++_purgePressure;
            return created;
        }

        internal void Enqueue(IndependentTimer timer)
        {
            _queue.Add(timer);
            ScheduleNext();
        }

        internal void Dequeue(IndependentTimer timer)
        {
            _queue.Remove(timer);
            ScheduleNext();
        }

        void ScheduleNext()
        {
            var now = GetRawTime();
            while (_queue.Count > 0 && _queue.Min.ExpirationTime <= now)
            {
                _queue.Min.Expire();
                _queue.Remove(_queue.Min);
            }
            if (_queue.Count > 0)
                ScheduleTimer(_queue.Min.ExpirationTime);
            else
                CancelTimer();
        }
    }
}
