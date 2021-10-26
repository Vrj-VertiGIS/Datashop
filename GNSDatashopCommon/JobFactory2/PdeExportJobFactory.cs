using System;
using System.Collections.Generic;
using System.Linq;
using GEOCOM.GNSD.Common.Model;

namespace GEOCOM.GNSD.Common.JobFactory
{
    /// <summary>
    /// Validates the data input and creates the export model for the Partial Data Export
    /// </summary>
    public class PdeExportJobFactory
    {
        /// <summary>
        /// Creates the job.
        /// </summary>
        /// <param name="profileGuid">The profile GUID.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <param name="perimeters">The perimeters.</param>
        /// <returns></returns>
        public TdeExportModel CreateJob(string profileGuid, OutputFormat outputFormat, IEnumerable<ExportPerimeter> perimeters)
        {
            if (string.IsNullOrEmpty(profileGuid))
                throw new ArgumentNullException("profileGuid");

            if (outputFormat != OutputFormat.fgdb && outputFormat != OutputFormat.pgdb && outputFormat != OutputFormat.DXF)
                throw new ArgumentException(string.Format("The output format defined is invalid. Possible options are fgdb, pgdb and DXF. You entered: {0}",
                    outputFormat));

            if (perimeters == null)
                throw new ArgumentNullException("perimeters");

            return new TdeExportModel
                        {
                            ProfileGuid = profileGuid, 
                            OutputFormat = outputFormat, 
                            Perimeters = perimeters.ToArray()
                        };
        }
    }
}