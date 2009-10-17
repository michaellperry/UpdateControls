using System;
using System.Diagnostics;
using System.Threading;

namespace UpdateControls
{
    public partial class Dependent
    {
        private static LocalDataStoreSlot _currentUpdateSlot = Thread.AllocateDataSlot();

        internal static Dependent GetCurrentUpdate()
        {
            return (Dependent)Thread.GetData(_currentUpdateSlot);
        }

        private static void SetCurrentUpdate(Dependent dependent)
        {
            Thread.SetData(_currentUpdateSlot, dependent);
        }

        static partial void ReportCycles()
        {
            Trace.WriteLine(string.Format("Cycle discovered during update: {0}", Environment.StackTrace));
        }
    }
}