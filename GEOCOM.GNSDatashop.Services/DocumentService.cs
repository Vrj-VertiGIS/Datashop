using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.ServiceModel;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSDatashop.Model.Documents;
using GEOCOM.GNSDatashop.Model.JobData;
using GEOCOM.GNSDatashop.ServiceContracts;

namespace GEOCOM.GNSDatashop.Services
{
    /// <summary>
    /// Class that defines the endpoint for the DocumentService functionality
    /// </summary>
    [ServiceBehavior(Namespace = "http://datashop.geocom.ch")]
    public class DocumentService : IDocumentService
    {
        #region Private Members

        /// <summary>
        /// Holds the logger for this instance
        /// </summary>
        private readonly IMsg log = new Msg(typeof(DocumentService));

        /// <summary>
        /// Holds the memory stream for returning streamed data
        /// </summary>
        private MemoryStream memStream;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentService"/> class.
        /// </summary>
        public DocumentService()
        {
            if (OperationContext.Current != null)
                OperationContext.Current.OperationCompleted += Current_OperationCompleted;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public DocumentDownload GetDocument(DocumentRequest request)
        {
            if (request == null) throw new FaultException("request cannot be null");

            try
            {
                var jobStore = new JobStore();
                var job = jobStore.GetById(request.JobId);

                this.log.DebugFormat("Found documents for UserId={0} JobId={1} FileName={2}", request.UserId, request.JobId, job.JobOutput);

                var document = new DocumentDownload
                {
                    JobId = job.JobId,
                    UserId = job.UserId,
                    FileName = job.JobOutput,
                };

                if (!File.Exists(job.JobOutput))
                {
                    this.log.WarnFormat("The file FileName={2} for userId={0} JobId={1} does not exists.", request.UserId, request.JobId, job.JobOutput);
                    document.Contents = Stream.Null;
                    document.Length = Stream.Null.Length;
                    document.FileExists = false;
                    return document;
                }


                using (var fileStream = File.Open(job.JobOutput, FileMode.Open, FileAccess.Read))
                {
					memStream = new MemoryStream();
                    fileStream.CopyTo(memStream);
                    memStream.Position = 0;
                }

                job.DownloadCount++;
                jobStore.Update(job);

                document.Contents = memStream;
                document.Length = memStream.Length;
                document.FileExists = true;

                return document;
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("Error downloading document", ex, request.JobId, request.UserId);

                throw new FaultException<Exception>(ex, "Fatal error during GetDocument");
            }
        }

        public DocumentDownload GetDocumentDatashopAddon(DocumentRequestDatashopAddon request)
        {
            DocumentRequest documentRequest = new DocumentRequest() { JobId = request.JobId, FileName = request.FileName, UserId = request.UserId };
            return GetDocument(documentRequest);
        }

        /// <summary>
        /// Saves the document.
        /// </summary>
        /// <param name="upload">The doc.</param>
        public void SaveDocument(DocumentUpload upload)
        {
            if (upload == null) throw new FaultException("upload cannot be null");

            try
            {
                var jobStore = new JobStore();
                var job = jobStore.GetById(upload.JobId);

                this.SaveDocumentToDisk(upload, job);
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat("Error saving document", ex, upload.JobId, upload.UserId);

                throw new FaultException<Exception>(ex, "Fatal error during SaveDocument");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the document to disk.
        /// </summary>
        /// <param name="upload">The doc.</param>
        /// <param name="job">The job.</param>
        private void SaveDocumentToDisk(DocumentUpload upload, Job job)
        {
            Assert.True(job != null, "job");

            var jobOutputDirectory = Path.GetDirectoryName(job.JobOutput);

            if (jobOutputDirectory != null)
            {
                if (!Directory.Exists(jobOutputDirectory))
                    throw new DirectoryNotFoundException(string.Format("Directory={0} doesn't exist", jobOutputDirectory));

                var fullPath = Path.Combine(jobOutputDirectory, upload.FileName);

                this.log.DebugFormat("Uploading document for UserId={0} JobId={1} FileName={2}", job.UserId, job.JobId, upload.FileName);

                using (upload.Contents)
                {
                    using (var fileStream = File.Create(fullPath, (int)upload.Length))
                    {
                        upload.Contents.CopyTo(fileStream);

                        fileStream.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the OperationCompleted event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Current_OperationCompleted(object sender, EventArgs e)
        {
            if (this.memStream != null)
            {
                this.memStream.Dispose();
                GC.Collect();
            }
        }
        #endregion
    }
}