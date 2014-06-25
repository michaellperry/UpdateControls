using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UpdateControls.Timers
{
    public class IndependentTimer : Independent
    {
        FloatingTimeZone _zone;
        DateTime _time;
        bool _expired;

        public DateTime ExpirationTime { get { return _time; } }

        public bool IsExpired
        {
            get
            {
                OnGet();
                return _expired;
            }
        }

        internal IndependentTimer(FloatingTimeZone zone, DateTime time)
        {
            _zone = zone;
            _time = time;
            _expired = _zone.GetStableTime() >= time;
        }

        public static IndependentTimer Get(FloatingTimeZone zone, DateTime time)
        {
            return zone.GetTimer(time);
        }

        public static IndependentTimer GetUtc(DateTime utc)
        {
            return FloatingTimeZone.Utc.GetTimer(utc);
        }

        protected override void GainDependent()
        {
            _zone.Enqueue(this, _expired);
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
