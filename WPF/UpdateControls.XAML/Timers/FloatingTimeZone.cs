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
        SortedSet<IndependentTimer> _forth = new SortedSet<IndependentTimer>(_comparer);
        SortedSet<IndependentTimer> _back = new SortedSet<IndependentTimer>(_comparer);
        DateTime _schedule;
        DateTime? _stable;
        FloatingDateTime _now;
        static Comparer<IndependentTimer> _comparer = Comparer<IndependentTimer>.Create((l, r) => Comparer<DateTime>.Default.Compare(l.ExpirationTime, r.ExpirationTime));

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
            _schedule = DateTime.MinValue;
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

        internal void Enqueue(IndependentTimer timer, bool expired)
        {
            (expired ? _back : _forth).Add(timer);
            ScheduleNext();
        }

        internal void Dequeue(IndependentTimer timer)
        {
            _forth.Remove(timer);
            _back.Remove(timer);
        }

        void ScheduleNext()
        {
            var now = GetStableTime();
            while (_back.Count > 0 && _back.Max.ExpirationTime > now)
            {
                var timer = _back.Min;
                _back.Remove(timer);
                _forth.Add(timer);
                timer.Expire(false);
            }
            while (_forth.Count > 0 && _forth.Min.ExpirationTime <= now)
            {
                var timer = _forth.Min;
                _forth.Remove(timer);
                _back.Add(timer);
                timer.Expire(true);
            }
            if (_forth.Count == 0 && _back.Count == 0)
            {
                if (_schedule != DateTime.MinValue)
                {
                    _schedule = DateTime.MinValue;
                    CancelTimer();
                }
            }
            else if (_forth.Count == 0)
            {
                if (_schedule != DateTime.MaxValue)
                {
                    _schedule = DateTime.MaxValue;
                    ScheduleTimer(TimeSpan.FromSeconds(1));
                }
            }
            else if (_forth.Min.ExpirationTime != _schedule)
            {
                _schedule = _forth.Min.ExpirationTime;
                ScheduleTimer(new TimeSpan(Math.Min((_schedule - now).Ticks, TimeSpan.FromSeconds(1).Ticks)));
            }
        }
    }
}
