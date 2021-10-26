using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ESRI.ArcGIS.Geodatabase;
using GEOCOM.Common;
using GEOCOM.GNSD.DatashopWorkflow.Config;
using GEOCOM.GNSD.DatashopWorkflow.GeoDataBase;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoAttachments
{
    /// <summary>
    /// Collection of geo-attachments with helper methods
    /// </summary>
    public class GeoAttachmentCollection : IEnumerable<IGeoAttachment>
    {
        private readonly GeoAttachmentsConfig.UrlAuth _urlAuthentication;
        private readonly List<IGeoAttachment> _geoAttachments = new List<IGeoAttachment>();
        private readonly Dictionary<string, int> _indexCache = new Dictionary<string, int>();

        public GeoAttachmentCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoAttachmentCollection"/> class.
        /// </summary>
        /// <param name="urlAuthentication">The URL authentication that will be used for URL requests</param>
        public GeoAttachmentCollection(GeoAttachmentsConfig.UrlAuth urlAuthentication)
        {
            _urlAuthentication = urlAuthentication;
        }

        /// <summary>
        /// Initials a geo-attachment from the <paramref name="feature"/> and adds it the collection.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="filePathFieldName">Name of the field of the <paramref name="feature"/> with path to the geo-attachment file.</param>
        /// <returns>The geo-attachment.</returns>
        public void AddFromFeature(IFeature feature, string filePathFieldName)
        {
            int filePathFieldIndex = FindFieldIndex(feature, filePathFieldName);
            var filePath = feature.Value[filePathFieldIndex] as string;

            var geoAttachment = CreateGeoAttachment(feature.OID, filePath);
            _geoAttachments.Add(geoAttachment);
        }

        /// <summary>
        /// Initials s geo-attachments from the <paramref name="features"/> and adds it the collection.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <param name="filePathFieldName">Name of the field of the <paramref name="features"/> with path to the geo-attachment file.</param>
        /// <returns>The geo-attachments.</returns>
        public void AddFromFeatures(IEnumerable<IFeature> features, string filePathFieldName)
        {
            foreach (var feature in features)
            {
                AddFromFeature(feature, filePathFieldName);
            }
        }

        /// <summary>
        /// Checks the actual size of all geo-attachments and compare it with the <paramref name="maxTotalSizeMB" />.
        /// </summary>
        /// <param name="maxTotalSizeMB">The max total size MB limit. If it is not a valid number, the check will be skipped.</param>
        /// <returns>Sum of the file sizes in bytes (B).</returns>
        /// <exception cref="System.ApplicationException">If size geo-attachment in the collection exceeds the  <paramref name="maxTotalSizeMB" />. MAX_SIZE_EXCEEDED is set to </exception>
        public long CheckTotalSize(string maxTotalSizeMB)
        {
            var totalSizeB = _geoAttachments.Sum(attachment => attachment.FileSize);

            double maxSizeMB; // in megabytes
            var maxSizeValid = double.TryParse(maxTotalSizeMB, out maxSizeMB);
            var checkMaxSize = maxSizeValid || maxSizeMB > 0;
            if (checkMaxSize)
            {
                const long bytesInMegaByte = 1048576; // 1048576 = 1014 * 1024 is conversion from bytes to megabytes.
                long maxSizeB = (long)maxSizeMB * bytesInMegaByte;
                if (maxSizeB < totalSizeB)
                {
                    var msg = string.Format("The maximal allowed size of {0:0.##} MB of geo-attachments was exceeded. The actual size was {1:0.##} MB.", maxSizeMB, totalSizeB / (double)bytesInMegaByte);
                    var exception = new GeoAttachmentsMaxSizeExceededException(msg)
                        {
                            ActualSize = totalSizeB,
                            MaxSize = maxSizeB
                        };
                    throw exception;
                }
            }

            return totalSizeB;
        }

        /// <summary>
        /// Copies all geo-attachment in the collection to the <paramref name="destinationFolder"/>.
        /// </summary>
        /// <param name="destinationFolder">The destination folder.</param>
        public IList<string> CopyAll(string destinationFolder)
        {
            try
            {
                if (_geoAttachments.Any())
                    CreateFolderIfNeeded(destinationFolder);

                var list = new List<string>();
                foreach (var geoAttachment in _geoAttachments)
                {
                    string attachmentPath = geoAttachment.FilePath;
                    string sourceFileName = Path.GetFileName(attachmentPath);
                    string destionationPath = Path.Combine(destinationFolder, sourceFileName);

                    CopyFile(attachmentPath, destionationPath, geoAttachment.FeatureId);
                    list.Add(destionationPath);
                }

                return list;
            }
            catch (Exception e)
            {
                var message = string.Format("An error occurred during copying of geo-attachments: {0}", e.Message);
                throw new Exception(message, e);
            }
        }

        #region public virtual unit testing methods

        /// internal virtual because of unit tests and mocking 
        public virtual int FindFieldIndex(IFeature feature, string fieldName)
        {
            string fullyQualifiedFieldName = ((feature.Table as IDataset).Name + ":" + fieldName).ToLower();
            int index;
            var found = _indexCache.TryGetValue(fullyQualifiedFieldName, out index);
            if (!found)
            {
                index = GeoDbOperation.FindFieldIndex(feature.Fields, fieldName);
                _indexCache[fullyQualifiedFieldName] = index;
            }
            return index;
        }

        // internal virtual because of unit tests and mocking 
        public virtual IGeoAttachment CreateGeoAttachment(int featureId, string filePath)
        {
            var fileSize = GetFileSize(featureId, filePath);
            var geoAttachment = new GeoAttachment(featureId, filePath, fileSize);
            return geoAttachment;
        }


        private long GetFileSize(int featureId, string filePath)
        {
            var filePathIsURL = IsPathUrl(filePath);
            if (filePathIsURL)
            {
                try
                {
                    var response = GetFileHttpWebResponse(filePath, "HEAD");
                    int contentLength;
                    var header = response.Headers.Get("Content-Length");
                    if (!int.TryParse(header, out contentLength))
                    {
                        var format = string.Format("Size of the geo-attachment file '{0}' for the feature id='{1}' could not be determined.", filePath,
                            featureId);
                        throw new ApplicationException(format);
                    }

                    return contentLength;
                }
                catch (Exception e)
                {
                    var message = string.Format("Could not access URL {0} - {1}", filePath, e.Message);
                    throw new ApplicationException(message, e);
                }
            }

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                var msg = string.Format("The geo-attachment file '{0}' for the feature id='{1}' was not found.", filePath,
                    featureId);
                throw new FileNotFoundException(msg);
            }
            var fileSize = fileInfo.Length;
            return fileSize;
        }

        private static bool IsPathUrl(string filePath)
        {
            return filePath.Trim().StartsWith("http://") || filePath.Trim().StartsWith("https://");
        }

        private HttpWebResponse GetFileHttpWebResponse(string filePath, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filePath);
            request.Accept = @"*/*";
            request.Method = method;

            var addCredentialsToRequest =
                _urlAuthentication != null &&
                !string.IsNullOrEmpty(_urlAuthentication.Password) &&
                !string.IsNullOrEmpty(_urlAuthentication.UserName) &&
                !string.IsNullOrEmpty(_urlAuthentication.Type);
            if (addCredentialsToRequest)
                request.Credentials = new CredentialCache
                {
                    {
                        new Uri(filePath),
                        _urlAuthentication.Type,
                        new NetworkCredential(_urlAuthentication.UserName, _urlAuthentication.Password)
                    }
                };

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        // internal virtual because of unit tests and mocking 
        public virtual void CreateFolderIfNeeded(string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);
        }

        // internal virtual because of unit tests and mocking 
        public virtual void CopyFile(string attachmentPath, string destionationPath, int featureId)
        {
            var filePathIsURL = IsPathUrl(attachmentPath);
            if (filePathIsURL)
            {
                try
                {
                    using (var response = GetFileHttpWebResponse(attachmentPath, "GET"))
                    {
                        using (var fileStream = File.Create(destionationPath))
                        {
                            var responseStream = response.GetResponseStream();
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Could not access URL " + attachmentPath, e);
                }
            }
            else
            {
                File.Copy(attachmentPath, destionationPath, true);
            }

        }

        #endregion

        public IEnumerator<IGeoAttachment> GetEnumerator()
        {
            return _geoAttachments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
