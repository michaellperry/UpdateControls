using System;
using System.Linq;
using System.Threading;
using UpdateControls.Fields;

namespace UpdateControls.UnitTest.MultithreadedData
{
    public class TargetThread
    {
        private SourceThread[] _sources;
        private Thread _thread;
        private Dependent<int> _total;

        public TargetThread(SourceThread[] sources)
        {
            _sources = sources;
            _thread = new Thread(ThreadProc);
            _total = new Dependent<int>(() => _sources.Sum(source => source.Value));
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Join()
        {
            _thread.Join();
        }

        public int Total
        {
            get
            {
                lock (this)
                    return _total;
            }
        }

        private void ThreadProc()
        {
            for (int i = 0; i < SourceThread.MaxValue; i++)
            {
                int total = Total;
                if (total < 0) // This will never happen, but we need to ensure that the property get is not optimized out.
                    throw new InvalidOperationException();
            }
        }
    }
}
