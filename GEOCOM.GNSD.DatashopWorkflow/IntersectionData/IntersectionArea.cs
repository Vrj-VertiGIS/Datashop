namespace GEOCOM.GNSD.DatashopWorkflow.IntersectionData
{
    /// <summary>
    /// Represents a result of 2D intersection.
    /// </summary>
    public class IntersectionArea : IntersectionGeometry
    {
        public IntersectionArea(string name, double area)
        {
            Name = name;
            Area = area;
        }

        /// <summary>
        /// Area size of an intersection of 2D geometries.
        /// </summary>
        public double Area { get; set; }


        public override void Add(IntersectionGeometry geometry)
        {
            IntersectionArea area = CheckTypeAndCast<IntersectionArea>(geometry);
            Area += area.Area;
        }

        public override int CompareTo(IntersectionGeometry geometry)
        {
            IntersectionArea area = CheckTypeAndCast<IntersectionArea>(geometry);
            return Area.CompareTo(area.Area);
        }
    }
}