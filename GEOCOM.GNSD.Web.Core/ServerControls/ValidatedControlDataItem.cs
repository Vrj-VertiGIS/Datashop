namespace GEOCOM.GNSD.Web.Core.ServerControls
{
    /// <summary>
    /// Data Item for validated controls
    /// </summary>
    public class ValidatedControlDataItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ValidatedControlDataItem"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ValidatedControlDataItem"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Visible { get; set; }
    }
}