using System.IO;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoAttachments
{
    public interface IGeoAttachment
    {
        /// <summary>
        /// Gets or sets the ArcGIS object id of the original feature.
        /// </summary>
        /// <value>
        /// The feature id.
        /// </value>
        int FeatureId { get; set; }

        /// <summary>
        /// Gets or sets the file path of the attachment.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        string FilePath { get; }

        /// <summary>
        /// Gets or sets the size of the attachment file in bytes (B).
        /// </summary>
        /// <value>
        /// The size of the file in bytes (B).
        /// </value>
        long FileSize { get; }
    }
}