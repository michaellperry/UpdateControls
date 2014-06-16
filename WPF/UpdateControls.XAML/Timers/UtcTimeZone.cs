using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UpdateControls.Timers
{
    class UtcTimeZone : IndependentTimeZone
    {
        DispatcherTimer _timer = new DispatcherTimer();
        DateTime? _schedule;

        public UtcTimeZone() { _timer.Tick += Expire; }

        public override DateTime GetRawTime() { return DateTime.UtcNow; }

        protected override void ScheduleTimer(DateTime time)
        {
            if (time != _schedule)
            {
                _schedule = time;
                _timer.Stop();
                _timer.Interval = new TimeSpan(Math.Min(TimeSpan.FromDays(3).Ticks, Math.Max(TimeSpan.FromMilliseconds(20).Ticks, (_schedule.Value - GetRawTime()).Ticks)));
                _timer.Start();
            }
        }

        protected override void CancelTimer()
        {
            _timer.Stop();
            _schedule = null;
        }

        void Expire(object sender, EventArgs args)
        {
            _timer.Stop();
            if (_schedule != null)
            {
                if (_schedule.Value <= GetRawTime())
                {
                    _schedule = null;
                    NotifyTimerExpired();
                }
                else
                    ScheduleTimer(_schedule.Value);
            }
        }
    }
}
