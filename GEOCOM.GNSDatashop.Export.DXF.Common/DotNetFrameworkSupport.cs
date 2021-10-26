using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class DotNetFrameworkSupport
    {

        public static void CollectGarbage()
        {
            long allocatedAfterGC = 0;
            var allocatedBeforeGC = GC.GetTotalMemory(false);
            GC.Collect();
            while (allocatedBeforeGC > (1.001 * (allocatedAfterGC = GC.GetTotalMemory(false))))
            {
                allocatedBeforeGC = allocatedAfterGC;
                GC.Collect();   // Continue collecting until no more than a 1%% gain of free memory
            }
        }
    }
}
