using System;
using System.Collections.Generic;
using System.Linq;
using GEOCOM.GNSD.Common.Model;

namespace GEOCOM.GNSD.Common.JobFactory2
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfExportJobFactory
    {
        /// <summary>
        /// Creates the job.
        /// </summary>
        /// <param name="exportName">Name of the export.</param>
        /// <param name="perimeters">The perimeters.</param>
        /// <returns></returns>
        public DxfExportModel CreateJob(string exportName, IEnumerable<ExportPerimeter> perimeters)
        {
            if (string.IsNullOrWhiteSpace(exportName))
                throw new ArgumentNullException("exportName");

            if (perimeters == null)
                throw new ArgumentNullException("perimeters");

            return new DxfExportModel
                        {
                            OutputFormat = OutputFormat.DXF,
                            DxfExportName = exportName,
                            Perimeters = perimeters.ToArray()
                        };
        }
    }
}