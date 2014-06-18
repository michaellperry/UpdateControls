using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.Timers
{
    public abstract class FloatingTimeSpan
    {
        protected readonly IndependentTimeZone _zone;
        protected readonly DateTime _zero;

        public IndependentTimeZone Zone { get { return _zone; } }
        public DateTime ZeroMoment { get { return _zero; } }
        public abstract TimeSpan Snapshot { get; }
        public int Days { get { return GetComponent(Snapshot.Days, new TimeSpan(Snapshot.Days, 0, 0, 0), TimeSpan.FromDays(1)); } }
        public int Hours { get { return GetComponent(Snapshot.Hours, new TimeSpan(Snapshot.Days, Snapshot.Hours, 0, 0), TimeSpan.FromHours(1)); } }
        public int Minutes { get { return GetComponent(Snapshot.Minutes, new TimeSpan(Snapshot.Days, Snapshot.Hours, Snapshot.Minutes, 0), TimeSpan.FromMinutes(1)); } }
        public int Seconds { get { return GetComponent(Snapshot.Seconds, new TimeSpan(Snapshot.Days, Snapshot.Hours, Snapshot.Minutes, Snapshot.Seconds), TimeSpan.FromSeconds(1)); } }

        protected abstract int GetComponent(int component, TimeSpan cut, TimeSpan increment);

        protected FloatingTimeSpan(IndependentTimeZone zone, DateTime zero)
        {
            _zone = zone;
            _zero = zero;
        }

        public override string ToString() { return Snapshot.ToString(); }

        protected void CheckZone(FloatingTimeSpan other)
        {
            if (_zone != other._zone)
                throw new ArgumentException("Cannot relate FloatingTimeSpan instances from two different time zones");
        }
    }
}
