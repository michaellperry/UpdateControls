using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UpdateControls.Timers
{
    public abstract class FloatingTimeZone
    {
        Dictionary<DateTime, WeakReference> _timers = new Dictionary<DateTime, WeakReference>();
        int _purgePressure;
        SortedSet<IndependentTimer> _queue = new SortedSet<IndependentTimer>(
            Comparer<IndependentTimer>.Create((l, r) => Comparer<DateTime>.Default.Compare(l.ExpirationTime, r.ExpirationTime)));
        DateTime? _schedule;
        DateTime? _stable;
        DateTime _skewMarker;
        FloatingDateTime _now;

        public static readonly FloatingTimeZone Utc = new UtcTimeZone();

        public FloatingDateTime Now { get { return _now; } }

        protected FloatingTimeZone()
        {
            _now = new FloatingDateTime(this);
        }

        public abstract DateTime GetRawTime();
        protected abstract void ScheduleTimer(TimeSpan delay);
        protected abstract void CancelTimer();

        public DateTime GetStableTime()
        {
            if (_stable == null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _stable = null), DispatcherPriority.Input);
                _stable = GetRawTime();
            }
            return _stable.Value;
        }

        protected void NotifyTimerExpired()
        {
            _schedule = null;
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
            var now = GetStableTime();
            if (now < _skewMarker && _skewMarker - now > TimeSpan.FromSeconds(1))
            {
                foreach (var reference in _timers.Values)
                {
                    var timer = reference.Target as IndependentTimer;
                    if (timer != null && now < timer.ExpirationTime)
                    {
                        timer.Expire(false);
                        if (timer.HasDependents)
                            _queue.Add(timer);
                    }
                }
            }
            while (_queue.Count > 0 && _queue.Min.ExpirationTime <= now)
            {
                _queue.Min.Expire(true);
                _queue.Remove(_queue.Min);
            }
            if (_queue.Count == 0)
            {
                _schedule = null;
                CancelTimer();
            }
            else if (_queue.Min.ExpirationTime != _schedule)
            {
                _schedule = _queue.Min.ExpirationTime;
                ScheduleTimer(new TimeSpan(Math.Min((_schedule.Value - now).Ticks, TimeSpan.FromSeconds(1).Ticks)));
            }
            _skewMarker = now;
        }
    }
}
