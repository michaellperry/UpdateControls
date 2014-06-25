using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.Timers
{
    public sealed class DroppingTimeSpan : FloatingTimeSpan
    {
        public override TimeSpan Snapshot { get { return _zero - _zone.GetStableTime(); } }

        internal DroppingTimeSpan(FloatingTimeZone zone, DateTime zero) : base(zone, zero) { }

        public DroppingTimeSpan Add(TimeSpan timespan) { return new DroppingTimeSpan(_zone, _zero + timespan); }
        public TimeSpan Add(RisingTimeSpan other) { return _zero - other.ZeroMoment; }
        public DroppingTimeSpan Subtract(TimeSpan timespan) { return Add(-timespan); }
        public TimeSpan Subtract(DroppingTimeSpan other) { return _zero - other._zero; }

        public static DroppingTimeSpan operator +(DroppingTimeSpan left, TimeSpan right) { return left.Add(right); }
        public static DroppingTimeSpan operator +(TimeSpan left, DroppingTimeSpan right) { return right.Add(left); }
        public static TimeSpan operator +(DroppingTimeSpan left, RisingTimeSpan right) { return left.Add(right); }
        public static DroppingTimeSpan operator -(DroppingTimeSpan left, TimeSpan right) { return left.Subtract(right); }
        public static TimeSpan operator -(DroppingTimeSpan left, DroppingTimeSpan right) { return left.Subtract(right); }

        public static bool operator ==(DroppingTimeSpan left, DroppingTimeSpan right) { return left.Equals(right); }
        public static bool operator !=(DroppingTimeSpan left, DroppingTimeSpan right) { return !(left == right); }
        public static bool operator <(DroppingTimeSpan left, DroppingTimeSpan right) { return left.CompareTo(right) < 0; }
        public static bool operator >(DroppingTimeSpan left, DroppingTimeSpan right) { return left.CompareTo(right) > 0; }
        public static bool operator <=(DroppingTimeSpan left, DroppingTimeSpan right) { return left.CompareTo(right) <= 0; }
        public static bool operator >=(DroppingTimeSpan left, DroppingTimeSpan right) { return left.CompareTo(right) >= 0; }

        public static bool operator ==(DroppingTimeSpan left, TimeSpan right) { return left.Equals(right); }
        public static bool operator ==(TimeSpan left, DroppingTimeSpan right) { return right == left; }
        public static bool operator !=(DroppingTimeSpan left, TimeSpan right) { return !(left == right); }
        public static bool operator !=(TimeSpan left, DroppingTimeSpan right) { return right != left; }
        public static bool operator <=(DroppingTimeSpan left, TimeSpan right) { return left.GetTimer(right).IsExpired; }
        public static bool operator <=(TimeSpan left, DroppingTimeSpan right) { return right >= left; }
        public static bool operator <(DroppingTimeSpan left, TimeSpan right) { return left <= right && left != right; }
        public static bool operator <(TimeSpan left, DroppingTimeSpan right) { return right > left; }
        public static bool operator >=(DroppingTimeSpan left, TimeSpan right) { return !(left < right); }
        public static bool operator >=(TimeSpan left, DroppingTimeSpan right) { return right <= left; }
        public static bool operator >(DroppingTimeSpan left, TimeSpan right) { return !(left <= right); }
        public static bool operator >(TimeSpan left, DroppingTimeSpan right) { return right < left; }

        public bool Equals(DroppingTimeSpan other) { return _zone == other._zone && _zero == other._zero; }
        public bool Equals(TimeSpan other) { return GetTimer(other).IsExpired && !GetTimer1(other).IsExpired; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is DroppingTimeSpan)
                return Equals((DroppingTimeSpan)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            throw new ArgumentException();
        }
        public override int GetHashCode() { return 31 * _zone.GetHashCode() + _zero.GetHashCode(); }

        public int CompareTo(DroppingTimeSpan other)
        {
            CheckZone(other);
            return _zero.CompareTo(other._zero);
        }
        public int CompareTo(TimeSpan other)
        {
            return !GetTimer(other).IsExpired ? 1 : GetTimer1(other).IsExpired ? -1 : 0;
        }
        public int CompareTo(object obj)
        {
            if (obj is DroppingTimeSpan)
                return CompareTo((DroppingTimeSpan)obj);
            if (obj is DateTime)
                return CompareTo((DateTime)obj);
            throw new ArgumentException();
        }

        IndependentTimer GetTimer(TimeSpan comparand) { return IndependentTimer.Get(_zone, _zero - comparand); }
        IndependentTimer GetTimer1(TimeSpan comparand) { return GetTimer(comparand.Subtract(TimeSpan.FromTicks(1))); }
        protected override int GetComponent(int component, TimeSpan cut, TimeSpan increment)
        {
            var next = Snapshot <= TimeSpan.Zero ? (-cut) - increment : cut - TimeSpan.FromTicks(1);
            IndependentTimer.Get(_zone, _zero - next).OnGet();
            IndependentTimer.Get(_zone, _zero - next - increment).OnGet();
            return component;
        }
    }
}
