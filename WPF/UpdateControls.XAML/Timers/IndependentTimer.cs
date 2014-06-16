using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UpdateControls.Timers
{
    public class IndependentTimer : Independent
    {
        DateTime _utc;
        bool _expired;
        static Dictionary<DateTime, WeakReference> _cache = new Dictionary<DateTime, WeakReference>();
        static int _cachePressure;
        static SortedSet<IndependentTimer> _queue = new SortedSet<IndependentTimer>(
            Comparer<IndependentTimer>.Create((l, r) => Comparer<DateTime>.Default.Compare(l._utc, r._utc)));
        static DateTime? _next;
        static DispatcherTimer _timer = new DispatcherTimer();

        public bool IsExpired
        {
            get
            {
                if (_expired)
                    return true;
                else if (DateTime.UtcNow >= _utc)
                {
                    _expired = true;
                    OnSet();
                    return true;
                }
                else
                {
                    OnGet();
                    return false;
                }
            }
        }

        static IndependentTimer()
        {
            _timer.Tick += (s, args) => SelectNext();
        }

        IndependentTimer(DateTime utc)
        {
            _utc = utc;
            _expired = DateTime.UtcNow >= utc;
        }

        public static IndependentTimer Get(DateTime utc)
        {
            if (_cache.ContainsKey(utc))
            {
                var cached = _cache[utc].Target as IndependentTimer;
                if (cached != null)
                    return cached;
            }
            if (2 * _cachePressure >= _cache.Count)
            {
                foreach (var key in _cache.Keys.ToList())
                    if (!_cache[key].IsAlive)
                        _cache.Remove(key);
                _cachePressure = 0;
            }
            var created = new IndependentTimer(utc);
            _cache[utc] = new WeakReference(created);
            ++_cachePressure;
            return created;
        }

        public static bool HasExpired(DateTime utc)
        {
            return IndependentTimer.Get(utc).IsExpired;
        }

        protected override void GainDependent()
        {
            if (!_expired)
            {
                _queue.Add(this);
                SelectNext();
            }
        }

        protected override void LoseDependent()
        {
            _queue.Remove(this);
            SelectNext();
        }

        static void SelectNext()
        {
            var now = DateTime.UtcNow;
            while (_queue.Count > 0 && _queue.Min._utc <= now)
            {
                _queue.Min._expired = true;
                _queue.Min.OnSet();
                _queue.Remove(_queue.Min);
            }
            if (_next != null && _next.Value <= now || _queue.Count == 0)
            {
                _next = null;
                _timer.Stop();
            }
            if (_queue.Count > 0 && _queue.Min._utc != _next)
            {
                _next = _queue.Min._utc;
                _timer.Stop();
                _timer.Interval = new TimeSpan(Math.Min(TimeSpan.FromDays(3).Ticks, Math.Max(TimeSpan.FromMilliseconds(20).Ticks, (_next.Value - now).Ticks)));
                _timer.Start();
            }
        }
    }
}
