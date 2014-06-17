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

        public UtcTimeZone() { _timer.Tick += Expire; }

        public override DateTime GetRawTime() { return DateTime.UtcNow; }

        protected override void ScheduleTimer(TimeSpan delay)
        {
            _timer.Stop();
            _timer.Interval = new TimeSpan(Math.Min(TimeSpan.FromDays(3).Ticks, Math.Max(TimeSpan.FromMilliseconds(20).Ticks, delay.Ticks)));
            _timer.Start();
        }

        protected override void CancelTimer()
        {
            _timer.Stop();
        }

        void Expire(object sender, EventArgs args)
        {
            _timer.Stop();
            NotifyTimerExpired();
        }
    }
}
