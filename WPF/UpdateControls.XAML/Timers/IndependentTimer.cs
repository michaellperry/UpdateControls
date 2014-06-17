using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UpdateControls.Timers
{
    public class IndependentTimer : Independent
    {
        IndependentTimeZone _zone;
        DateTime _time;
        bool _expired;
        static Independent _exact = new Independent();
        static bool _exactScheduled;

        public DateTime ExpirationTime { get { return _time; } }

        public bool IsExpired
        {
            get
            {
                OnGet();
                return _expired;
            }
        }

        public bool IsExact
        {
            get
            {
                if (_zone.GetStableTime() == _time)
                {
                    _exact.OnGet();
                    if (!_exactScheduled)
                    {
                        _exactScheduled = true;
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            _exactScheduled = false;
                            _exact.OnSet();
                        }), DispatcherPriority.Input);
                    }
                    return true;
                }
                else
                    return false;
            }
        }

        internal IndependentTimer(IndependentTimeZone zone, DateTime time)
        {
            _zone = zone;
            _time = time;
            _expired = _zone.GetStableTime() >= time;
        }

        public static IndependentTimer Get(IndependentTimeZone zone, DateTime time)
        {
            return zone.GetTimer(time);
        }

        public static IndependentTimer Get(DateTime utc)
        {
            return IndependentTimeZone.Utc.GetTimer(utc);
        }

        protected override void GainDependent()
        {
            if (!_expired)
                _zone.Enqueue(this);
        }

        protected override void LoseDependent()
        {
            _zone.Dequeue(this);
        }

        internal void Expire(bool expire)
        {
            if (_expired != expire)
            {
                _expired = expire;
                OnSet();
            }
        }
    }
}
