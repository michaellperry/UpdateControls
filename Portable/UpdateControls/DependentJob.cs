using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls
{
    public class DependentJob : IUpdatable, IDisposable
    {
        Dependent _dependent;
        bool _running;

        public DependentJob(Action action)
        {
            _dependent = new Dependent(action);
            _dependent.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
        }

        public void Start()
        {
            if (_dependent == null)
                throw new InvalidOperationException("Cannot restart DependentJob");
            _running = true;
            UpdateScheduler.ScheduleUpdate(this);
        }

        public void Stop()
        {
            _running = false;
            _dependent.Dispose();
            _dependent = null;
        }

        public void Dispose() { Stop(); }

        void IUpdatable.UpdateNow()
        {
            if (_running)
                _dependent.OnGet();
        }
    }
}
