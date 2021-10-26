using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.Pde;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.DataWorkflow
{
    /// <summary>
    /// Base class for data extraction workflows, includes reusable methods for extracting extents, generating output file names
    /// and selecting the correct file extension for the type of export
    /// </summary>
    public abstract class DataWorkflowBase : DatashopWorkflowBase
    {
        #region Protected Abstract (Template) Methods

        /// <summary>
        /// Processes this instance.
        /// </summary>
        protected abstract override void Process();

        #endregion

        #region Protected Concrete Methods

        /// <summary>
        /// Gets the job extents.
        /// </summary>
        /// <returns></returns>
        protected override List<IGeometry> GetJobExtents()
        {
            var extractor = new DataExtractor(DataItem.JobDescriptionModel as TdeExportModel);

            return extractor.GetJobExtentList();         
        }
       
        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected string GetOutputFileName(int index)
        {
            try
            {
                var fileName = string.Format("db_{0}_{1}{2}", DataItem.JobId, index, this.GetFileExtension());

                var fileDir = Path.GetDirectoryName(DataItem.JobOutput);

                if (fileDir != null)
                    return Path.Combine(fileDir, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetOutputFileName failed with index: {0}", index), ex);
            }

            throw new Exception("Invalid file directory");
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <returns></returns>
        protected string GetFileExtension()
        {
            try
            {
                var outputFormat = ((TdeExportModel)DataItem.JobDescriptionModel).OutputFormat;

                switch (outputFormat)
                {
                    case OutputFormat.fgdb:
                        return ".gdb";
                    case OutputFormat.pgdb:
                        return ".mdb";
                    case OutputFormat.DXF:
                        return ".dxf";
                    case OutputFormat.None:
                        throw new ArgumentException(string.Format("targetformat for job {0} is None. Unable to perform extraction", DataItem.JobId));
                    case OutputFormat.PDF:
                        return ".pdf";
                    default:
                        throw new ArgumentException(string.Format("Job {0} has an unsupported targetformat: {1}", DataItem.JobId, outputFormat));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetFileExtension failed", ex);
            }
        } 

        #endregion
    }
}