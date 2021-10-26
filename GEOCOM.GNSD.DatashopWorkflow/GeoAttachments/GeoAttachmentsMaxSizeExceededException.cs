using System;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoAttachments
{
    /// <summary>
    /// A custom single purpose exception
    /// </summary>
    public class GeoAttachmentsMaxSizeExceededException : ApplicationException
    {
        public GeoAttachmentsMaxSizeExceededException(string msg):base(msg)
        {
        }

        /// <summary>
        /// Gets or sets the maximal size in Bytes.
        /// </summary>
        /// <value>
        /// The size in Bytes.
        /// </value>
        public long MaxSize { get; set; }

        /// <summary>
        /// Gets or sets the actual size in Bytes.
        /// </summary>
        /// <value>
        /// The size in Bytes.
        /// </value>
        public long ActualSize { get; set; }
    }
}