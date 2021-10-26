using System;
using System.IO;
using System.Net;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoAttachments
{
    /// <summary>
    /// Data class containing geo-attachment data
    /// </summary>
    public class GeoAttachment : IGeoAttachment
    {
        /// <summary>
        /// Gets or sets the ArcGIS object id of the original feature.
        /// </summary>
        /// <value>
        /// The feature id.
        /// </value>
        public int FeatureId { get; set; }

        /// <summary>
        /// Gets or sets the file path of the attachment.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the size of the attachment file in bytes (B).
        /// </summary>
        /// <value>
        /// The size of the file in bytes (B).
        /// </value>
        public long FileSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoAttachment" /> class.
        /// </summary>
        /// <param name="featureId">The ArcGIS object id of the original feature.</param>
        /// <param name="filePath">The file path of the attachment.</param>
        /// <param name="fileSize">Size of the file.</param>
        /// <exception cref="System.ArgumentException">If <paramref name="filePath" /> is null or empty.</exception>
        /// <exception cref="System.IO.FileNotFoundException">If <paramref name="filePath" /> is non-existing path.</exception>
        public GeoAttachment(int featureId, string filePath, long fileSize)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                var msg = string.Format("The file path to a geo-attachment is not set on the feature id='{0}'.", featureId);
                throw new ArgumentException(msg);
            }
            
            FeatureId = featureId;
            FilePath = filePath;
            FileSize = fileSize;
        }

    }
}
