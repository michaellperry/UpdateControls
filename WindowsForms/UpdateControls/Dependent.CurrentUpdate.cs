using System;
using System.Diagnostics;

namespace UpdateControls
{
    public partial class Dependent
    {
        static partial void ReportCycles()
        {
            Trace.WriteLine(string.Format("Cycle discovered during update: {0}", Environment.StackTrace));
        }
    }
}