using System;
using System.Diagnostics;

namespace UpdateControls
{
    public partial class Dependent
    {
        static partial void ReportCycles()
        {
            try
            {
                throw new InvalidOperationException("Cycle discovered during update.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}