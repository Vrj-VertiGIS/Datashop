using System;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.Workflow.Interfaces;

namespace GEOCOM.GNSD.JobEngine
{
    public class JobProcessor
    {
        // Log4Net instance
        private readonly IMsg _log;

        public JobProcessor(long jobId)
        {
            DatashopLogInitializer.Initialize(jobId.ToString());
            _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// The job with the passed jobId will be executed.
        /// </summary>
        public void ExecuteJob(long jobId, bool resetJobBeforeProcessing)
        {
            try
            {
                // Initialize the license once at the beginning of job execution
                ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Server);
                _log.Info("License acquired");

                var jobStore = new JobStore();
                var job = jobStore.GetById(jobId);
                _log.Info($"Processing JobId={jobId}, Status={job.State}, Step={job.Step}, IsActive={job.IsActive}");

                if (resetJobBeforeProcessing)
                {
                    _log.Info("Restarting the job workflow before processing");
                    job.State = 0;
                    job.Step = 0;
                    job.IsActive = true;
                    jobStore.Update(job);
                    _log.Info($"Processing JobId={jobId}, State={job.State}, Step={job.Step}, IsActive={job.IsActive}");
                }
                
                IWorkflow datashopWorkflow = DatashopWorkflowFactory.CreateWorkflowByJobId(jobId, true);
                datashopWorkflow.Run();

            }
            catch (Exception e)
            {
                _log.Error($"Job execution failed. JobId={jobId}. Message: {e.Message}", e);
            }
        }

        //private static bool CheckAndInitializeLicense(IAoInitialize engineInitialize, esriLicenseProductCode productCode)
        //{
        //    esriLicenseStatus licenseStatus = engineInitialize.IsProductCodeAvailable(productCode);

        //    if (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized)
        //    {
        //        return true;
        //    }

        //    if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
        //    {
        //        esriLicenseStatus status =
        //            engineInitialize.Initialize(productCode);

        //        return status == esriLicenseStatus.esriLicenseCheckedOut;
        //    }

        //    return false;
        //}
    }
}
