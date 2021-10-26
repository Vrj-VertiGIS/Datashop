using System.Runtime.Serialization;

namespace GEOCOM.GNSDatashop.Model
{
    /// <summary>
    /// Model class for reasons
    /// </summary>
    [DataContract]
	public class Reason
	{
        /// <summary>
        /// Gets or sets the reason id.
        /// </summary>
        /// <value>
        /// The reason id.
        /// </value>
		[DataMember]
        public virtual long ReasonId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [period date required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [period date required]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public virtual bool PeriodDateRequired { get; set; }
	}
}