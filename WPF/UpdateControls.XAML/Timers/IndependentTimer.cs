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

        public DateTime ExpirationTime { get { return _time; } }

        public bool IsExpired
        {
            get
            {
                if (_expired)
                    return true;
                else
                {
                    OnGet();
                    return false;
                }
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
