using System.IO;
using System.Web;
using GEOCOM.GNSD.Web.Core.Service;
using GEOCOM.GNSDatashop.Model.Documents;
using GEOCOM.GNSDatashop.Model.JobData;

namespace GEOCOM.GNSD.Web.Core.DocumentStreaming
{
    /// <summary>
    /// Instances of this class offers functionality to stream results of finished jobs to a response 
    /// </summary>
    public class DocumentStreamer
    {
        #region Private members

        private readonly Job _job;
        private readonly HttpResponse _targetResponse;

        #endregion

        #region Public methods

        /// <summary>
        /// Creates an instance of the DocumentStreamer class
        /// </summary>
        /// <param name="job">Job whose result will be downloaded.</param>
        /// <param name="targetResponse">Response in which to stream the downloaded files</param>
        public DocumentStreamer(Job job, HttpResponse targetResponse)
        {
            _job = job;
            _targetResponse = targetResponse;
        }

        /// <summary>
        /// Request a document from the DocumentService and stream it to the target response
        /// </summary>
        public void CopyDocumentToRespose()
        {
            ClearResponse();
            RequestJobFileAndCopyItToResponse();
            SetResponseHeaders();
            
        }

        #endregion

        #region Private methods

        private void ClearResponse()
        {
            _targetResponse.Clear();
            _targetResponse.ClearHeaders();
            _targetResponse.ClearContent();
            _targetResponse.Buffer = true;
        }
       

        private void RequestJobFileAndCopyItToResponse()
        {
            using (Stream inputStream = GetResponseStreamForJob())
            {
                using (Stream outputStream = _targetResponse.OutputStream)
                {
                    inputStream.CopyTo(outputStream);
                }
            }
        }

        private void SetResponseHeaders()
        {
            string attachFileName = "job_" + _job.JobId + ".zip";
            _targetResponse.AddHeader("Content-Disposition", "attachment; filename=" + attachFileName);
            _targetResponse.ContentType = "application/zip";
        }


        private Stream GetResponseStreamForJob()
        {
            var request = new DocumentRequest { JobId = _job.JobId, UserId = _job.UserId, FileName = _job.JobOutput };
            var response = DatashopService.Instance.DocumentService.GetDocument(request);

            if (!response.FileExists)
            {
                throw new FileNotFoundException($"File '{response.FileName}' for the job {response.JobId} does not exists on the server");
            }
            Stream responseStream = response.Contents;
            return responseStream;
        }

        #endregion
    }
}
