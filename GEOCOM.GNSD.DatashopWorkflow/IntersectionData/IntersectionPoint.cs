namespace GEOCOM.GNSD.DatashopWorkflow.IntersectionData
{
    /// <summary>
    /// Represents a result of 0D intersection.
    /// </summary>
    public class IntersectionPoint : IntersectionGeometry
    {

        public IntersectionPoint(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Not implemented - points have no geometric value.
        /// </summary>
        /// <param name="geometry"></param>
        public override void Add(IntersectionGeometry geometry)
        {
            CheckTypeAndCast<IntersectionPoint>(geometry);
            // does not make sence for points
        }

        /// <summary>
        /// Always returns zero - points are equal.
        /// </summary>
        public override int CompareTo(IntersectionGeometry geometry)
        {
            CheckTypeAndCast<IntersectionPoint>(geometry);
            return 0;
        }
    }
}