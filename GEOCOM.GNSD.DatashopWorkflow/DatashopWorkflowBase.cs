using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.GeoAttachments;
using GEOCOM.GNSD.DatashopWorkflow.GeoDataBase;
using GEOCOM.GNSD.DatashopWorkflow.Mailer;
using GEOCOM.GNSD.DBStore.Archive;
using GEOCOM.GNSD.Workflow;
using GEOCOM.GNSD.Workflow.Interfaces;
using GEOCOM.GNSDatashop.Model.DatashopWorkflow;
using Path = System.IO.Path;
using System.Linq;

namespace GEOCOM.GNSD.DatashopWorkflow
{
    public abstract class DatashopWorkflowBase : WorkflowBase<DatashopWorkflowDataItem>
    {
        private readonly IMsg _logger = DatashopWorkflowDataItem.Logger;

        protected sealed override void DefineWorkflow(IWorkflowDefinition workflowDefinition)
        {
            // add steps
            workflowDefinition.AddLast((int)PlotJobSteps.PreProcess, PreProcess);
            workflowDefinition.AddLast((int)PlotJobSteps.Process, Process);
            workflowDefinition.AddLast((int)PlotJobSteps.WaitForDocuments, Barrier, SkipWaitForDocuments, "Waiting for additional documents");
            workflowDefinition.AddLast((int)PlotJobSteps.Package, Package);
            workflowDefinition.AddLast((int)PlotJobSteps.Send, Send);
            workflowDefinition.AddLast((int)PlotJobSteps.JobDone, Barrier, "Job done");
            workflowDefinition.AddLast((int)PlotJobSteps.Archive, Archive);
        }

        /// <summary>
        /// If overridden, creates an instance of the <see cref="IWorkflowStepInterceptor" /> interface
        /// that is used to influence workflow processing.
        /// </summary>
        /// <returns></returns>
        protected override IWorkflowStepInterceptor GetWorkflowInterceptor()
        {
            var allRestartableStepNames = GetAllStepIdNames().Select(stepIdName => stepIdName.Name).ToArray();
            var workflowStepInterceptor = new DatashopWorkflowStepInterceptor(allRestartableStepNames, DataItem.WorkflowInterceptionSettings);
            return workflowStepInterceptor;
        }

        protected override void OnError(Exception e)
        {
            Utils.Utils.OnError(e, DataItem.JobId, GetType());
        }

        protected override void OnStoppedAfterStep(IWorkflowStep workflowStep)
        {
            var stepStopCriterionConfig = DatashopWorkflowConfig.Instance.WorkflowInterceptionSettings.StopCriteria.
                                                             SingleOrDefault(
                                                                 criterion =>
                                                                 criterion.StopAfterStepName.Equals(workflowStep.Name, StringComparison.OrdinalIgnoreCase));
            if (stepStopCriterionConfig == null || string.IsNullOrEmpty(stepStopCriterionConfig.MailRecipients))
                return;

            MailClient mailClient = new MailClient(_logger, DataItem.Variables);
            mailClient.SendJobStoppedAfterStepMail(stepStopCriterionConfig, stepStopCriterionConfig.MailRecipients);
        }

        #region Workflow steps

        private void PreProcess()
        {
            _logger.InfoFormat("PreProcess called for JobId={0}.", DataItem.JobId);
            var jobExtents = GetJobExtents();

            _logger.InfoFormat("Extracting intersection data.");
            Dictionary<string, string> extractedIntersectionData = GeoDbOperation.ExtractIntersectionData(DataItem.Extraction, jobExtents);
            FillPropertiesAndVariables(extractedIntersectionData);

            _logger.InfoFormat("Calculating center point.");
            IPoint center = GeoDbOperation.GetCenterPoint(jobExtents, DataItem.CenterArea.CenterAreaType);
            FillPropertiesAndVariables(center);

            CreateJobOutputDirectory();

            _logger.InfoFormat("Checking for affected data owners.");
            List<AffectedDataOwner> affectedDataOwners = GeoDbOperation.GetAffectedDataOwners(DataItem.NotificationDataBase, jobExtents);
            NotifyDataOwners(affectedDataOwners);

            _logger.InfoFormat("Writing job extents to geo database.");
            GeoDbOperation.WriteJobExtents(DataItem.ExtentDataBase, jobExtents, DataItem.JobId);

            _logger.InfoFormat("Copying geo-attachments.");
            FindAndCopyGeoAttachments(jobExtents);
        }

        protected abstract void Process();

        private bool SkipWaitForDocuments()
        {
            return (DataItem.SurrogateJob == null) || !DataItem.SurrogateJob.StopAfterProcess;
        }

        private void Package()
        {
            _logger.InfoFormat("Package called for JobId={0}", DataItem.JobId);

            DocumentZipper zipper = new DocumentZipper(DataItem.JobdocumentsDirectory);
            var zippedDocuments = zipper.ZipGeneratedDocuments(DataItem.JobOutput);

            //LogToJobLog("Packing {0} items:", zippedDocuments.Length);

            //foreach (var zippedDocument in zippedDocuments)
            //{
            //    LogToJobLog(zippedDocument);
            //}
        }

        private void Send()
        {
            _logger.InfoFormat("Send called for JobId={0}", DataItem.JobId);
            NotifyJobOwner();
        }

        private void Archive()
        {
            _logger.DebugFormat("Archive called for JobId={0}", DataItem.JobId);
            JobArchiver jobArchiver = new JobArchiver();
            jobArchiver.Archive(DataItem.JobId);
            DataItem.IsArchived = true;
        }

        #endregion

        #region PreProcess methods

        protected abstract List<IGeometry> GetJobExtents();

        private void CreateJobOutputDirectory()
        {
            string fileDir = Path.Combine(GnsDatashopCommonConfig.Instance.Directories.ExportDirectory, string.Format("Job_{0:d}", DataItem.JobId));
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            DataItem.JobOutput = Path.Combine(fileDir, string.Format("Documents_{0:d}.zip", DataItem.JobId));
        }

        private void FillPropertiesAndVariables(IPoint center)
        {
            DataItem.CenterAreaX = center.X;
            DataItem.CenterAreaY = center.Y;
            DataItem.Variables["job_center_area_x"] = center.X.ToString(DataItem.CenterArea.DisplayFormat);
            DataItem.Variables["job_center_area_y"] = center.Y.ToString(DataItem.CenterArea.DisplayFormat);
            _logger.DebugFormat(
                "Setting variables job_center_area_x={0} and job_center_area_y={1}",
                DataItem.Variables["job_center_area_x"],
                DataItem.Variables["job_center_area_y"]);
        }

        private void FillPropertiesAndVariables(Dictionary<string, string> extractedIntersectionData)
        {
            foreach (KeyValuePair<string, string> keyValuePair in extractedIntersectionData)
            {
                string propertyName = keyValuePair.Key;
                string propertyValue = keyValuePair.Value;

                PropertyInfo propertyInfo = typeof(DatashopWorkflowDataItem).GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(DataItem, propertyValue, null);
                    string variableName = string.Format("job_{0}", propertyName).ToLower();
                    DataItem.Variables[variableName] = propertyValue;
                    _logger.DebugFormat("Setting variable {0}={1}", variableName, propertyValue);
                }
                else
                {
                    _logger.DebugFormat("Property {0} not found in DataItem. Can't set variable!", propertyName);
                }
            }
        }

        private void NotifyDataOwners(IEnumerable<AffectedDataOwner> notifyExtentOwnerInfoList)
        {
            foreach (AffectedDataOwner affectedDataOwner in notifyExtentOwnerInfoList)
            {
                if (affectedDataOwner.HasToBeNotified)
                {
                    IList<string> extentsDesc = affectedDataOwner.GetAllExtentDescriptions();

                    // Do send the mail
                    MailClient mailClient = new MailClient(_logger, DataItem.Variables);

                    mailClient.SendDataOwnerNotifyMail(DataItem.JobId, affectedDataOwner.Owner, DataItem.User, extentsDesc);
                    LogToJobLog("Notifying owner {0} {1} on email {2}.", DataItem.User.FirstName, DataItem.User.LastName, DataItem.User.Email);
                }
            }
        }

        private void FindAndCopyGeoAttachments(List<IGeometry> jobExtents)
        {
            if (DatashopWorkflowConfig.Instance.GeoAttachmentsConfig == null)
            {
                LogToJobLog("Skipping geo-attachments: no configuration found.");
                return;
            }
            bool disabledByConfig = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.Mode == GeoAttachmentMode.Never;
            bool disabledByUser = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.Mode == GeoAttachmentMode.User && !DataItem.Job.GeoAttachmentsEnabled;

            if (disabledByConfig || disabledByUser)
                return;

            var workspacePath = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.Path;
            var featureClassName = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.FeatureClass;
            var filePathColumn = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.FilePathColumn;
            var maxSize = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.MaxSize;
            var urlAuthentication = DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.UrlAuthentication;
           

            var geoAttachmentsFeatures = GeoDbOperation.FindIntersectingFeatures(jobExtents, workspacePath, featureClassName);
            if (geoAttachmentsFeatures.Length == 0)
            {
                LogToJobLog("No geo-attachements found in the feature class '{0}'.", featureClassName, filePathColumn);
                return;
            }

            LogToJobLog("Copying geo-attachment:");
            LogToJobLog("Workspace: {0}", workspacePath);
            LogToJobLog("Feature class: {0}[{1}]", featureClassName, filePathColumn);

            var geoAttachmentCollection = new GeoAttachmentCollection(urlAuthentication);
            geoAttachmentCollection.AddFromFeatures(geoAttachmentsFeatures, filePathColumn);

            double totalSizeMB = -1;
            try
            {
                totalSizeMB = geoAttachmentCollection.CheckTotalSize(maxSize) / (double)(1024 * 1024); // 1024 * 1024 is conversion from bytes B to mega bytes MB
            }
            catch (GeoAttachmentsMaxSizeExceededException e)
            {
                MailClient mailClient = new MailClient(_logger, DataItem.Variables);
                var actualSize = e.ActualSize / (double)(1024 * 1024);
                mailClient.SendMaxGeoAttachemntSizeExceeded(maxSize, actualSize.ToString("0.##"));

                throw;
            }

            LogToJobLog("Total size = {0:0.##} MB in {1} files.", totalSizeMB, geoAttachmentCollection.Count());
            string destinationFolder = Path.Combine(
                Path.GetDirectoryName(DataItem.JobOutput),
                DatashopWorkflowConfig.Instance.GeoAttachmentsConfig.DirectoryName ?? string.Empty);
            geoAttachmentCollection.CopyAll(destinationFolder);
            LogToJobLog("feature id; file size; path to the geo-attachment");
            foreach (var attch in geoAttachmentCollection)
            {
                LogToJobLog("id={2}; {1:0.##}KB; {0}", attch.FilePath, attch.FileSize / (double)1024, attch.FeatureId);
            }
        }

        #endregion

        #region Send methods

        private void NotifyJobOwner()
        {
            // Send info messages (eMails) to job creator and others...
            MailClient mailClient = new MailClient(_logger, DataItem.Variables);
            if (DataItem.SurrogateJob != null)
            {
                switch (DatashopWorkflowConfig.Instance.RepresentativeJob.Recipient)
                {
                    case RepresentativeJobRecipient.Selected:
                        mailClient.SendPlotFinishedMail(DataItem.JobGuid, DataItem.User, true);
                        break;

                    case RepresentativeJobRecipient.Representative:
                        mailClient.SendPlotFinishedMail(DataItem.JobGuid, DataItem.SurrogateUser, true);
                        break;
                    case RepresentativeJobRecipient.Both:
                        mailClient.SendPlotFinishedMail(DataItem.JobGuid, DataItem.User, true);
                        mailClient.SendPlotFinishedMail(DataItem.JobGuid, DataItem.SurrogateUser, true);
                        break;
                }
            }
            else
            {
                mailClient.SendPlotFinishedMail(DataItem.JobGuid, DataItem.User, false);
            }
        }

        protected void LogToJobLog(string format, params object[] args)
        {
            JobLogStore jobLogStore = new JobLogStore();
            jobLogStore.Add(DataItem.Job, format, args);
        }

        #endregion
    }
}
