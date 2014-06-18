using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.Timers
{
    public sealed class RisingTimeSpan : FloatingTimeSpan, IEquatable<RisingTimeSpan>, IEquatable<TimeSpan>, IComparable<RisingTimeSpan>, IComparable<TimeSpan>
    {
        public override TimeSpan Snapshot { get { return _zone.GetStableTime() - _zero; } }

        internal RisingTimeSpan(FloatingTimeZone zone, DateTime zero) : base(zone, zero) { }

        public RisingTimeSpan Add(TimeSpan timespan) { return new RisingTimeSpan(_zone, _zero - timespan); }
        public TimeSpan Add(DroppingTimeSpan other) { return other.ZeroMoment - _zero; }
        public RisingTimeSpan Subtract(TimeSpan timespan) { return Add(-timespan); }
        public TimeSpan Subtract(RisingTimeSpan other) { return other._zero - _zero; }

        public static RisingTimeSpan operator +(RisingTimeSpan left, TimeSpan right) { return left.Add(right); }
        public static RisingTimeSpan operator +(TimeSpan left, RisingTimeSpan right) { return right.Add(left); }
        public static TimeSpan operator +(RisingTimeSpan left, DroppingTimeSpan right) { return left.Add(right); }
        public static RisingTimeSpan operator -(RisingTimeSpan left, TimeSpan right) { return left.Subtract(right); }
        public static TimeSpan operator -(RisingTimeSpan left, RisingTimeSpan right) { return left.Subtract(right); }

        public static bool operator ==(RisingTimeSpan left, RisingTimeSpan right) { return left.Equals(right); }
        public static bool operator !=(RisingTimeSpan left, RisingTimeSpan right) { return !(left == right); }
        public static bool operator <(RisingTimeSpan left, RisingTimeSpan right) { return left.CompareTo(right) < 0; }
        public static bool operator >(RisingTimeSpan left, RisingTimeSpan right) { return left.CompareTo(right) > 0; }
        public static bool operator <=(RisingTimeSpan left, RisingTimeSpan right) { return left.CompareTo(right) <= 0; }
        public static bool operator >=(RisingTimeSpan left, RisingTimeSpan right) { return left.CompareTo(right) >= 0; }

        public static bool operator ==(RisingTimeSpan left, TimeSpan right) { return left.Equals(right); }
        public static bool operator ==(TimeSpan left, RisingTimeSpan right) { return right == left; }
        public static bool operator !=(RisingTimeSpan left, TimeSpan right) { return !(left == right); }
        public static bool operator !=(TimeSpan left, RisingTimeSpan right) { return right != left; }
        public static bool operator >=(RisingTimeSpan left, TimeSpan right) { return left.GetTimer(right).IsExpired; }
        public static bool operator >=(TimeSpan left, RisingTimeSpan right) { return right <= left; }
        public static bool operator <(RisingTimeSpan left, TimeSpan right) { return !(left >= right); }
        public static bool operator <(TimeSpan left, RisingTimeSpan right) { return right > left; }
        public static bool operator <=(RisingTimeSpan left, TimeSpan right) { return left < right || left == right; }
        public static bool operator <=(TimeSpan left, RisingTimeSpan right) { return right >= left; }
        public static bool operator >(RisingTimeSpan left, TimeSpan right) { return left >= right && left != right; }
        public static bool operator >(TimeSpan left, RisingTimeSpan right) { return right < left; }

        public bool Equals(RisingTimeSpan other) { return _zone == other._zone && _zero == other._zero; }
        public bool Equals(TimeSpan other) { return IndependentTimer.Get(_zone, _zero + other).IsExact; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is RisingTimeSpan)
                return Equals((RisingTimeSpan)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            throw new ArgumentException();
        }
        public override int GetHashCode() { return 31 * _zone.GetHashCode() + _zero.GetHashCode(); }

        public int CompareTo(RisingTimeSpan other)
        {
            CheckZone(other);
            return other._zero.CompareTo(_zero);
        }
        public int CompareTo(TimeSpan other)
        {
            var timer = GetTimer(other);
            return timer.IsExact ? 0 : timer.IsExpired ? 1 : -1;
        }
        public int CompareTo(object obj)
        {
            if (obj is RisingTimeSpan)
                return CompareTo((RisingTimeSpan)obj);
            if (obj is DateTime)
                return CompareTo((DateTime)obj);
            throw new ArgumentException();
        }

        IndependentTimer GetTimer(TimeSpan comparand) { return IndependentTimer.Get(_zone, _zero + comparand); }
        protected override int GetComponent(int component, TimeSpan cut, TimeSpan increment)
        {
            var next = Snapshot >= TimeSpan.Zero ? cut + increment : (-cut) + TimeSpan.FromTicks(1);
            IndependentTimer.Get(_zone, _zero + next).OnGet();
            return component;
        }
    }
}
