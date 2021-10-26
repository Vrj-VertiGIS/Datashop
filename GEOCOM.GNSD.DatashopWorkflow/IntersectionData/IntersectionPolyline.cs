namespace GEOCOM.GNSD.DatashopWorkflow.IntersectionData
{
    /// <summary>
    /// Represents a result of 1D intersection.
    /// </summary>
    public class IntersectionPolyline : IntersectionGeometry
    {
      
        public IntersectionPolyline(string name, double area)
        {
            Name = name;
            Lenght = area;
        }
        
        /// <summary>
        /// Lenght of an intersection of a 1D geometry and 2D geometries.
        /// </summary>
        public double Lenght { get; set; }

        public override void Add(IntersectionGeometry geometry)
        {

            IntersectionPolyline polyline = CheckTypeAndCast<IntersectionPolyline>(geometry);
            Lenght += polyline.Lenght;
        }

        public override int CompareTo(IntersectionGeometry geometry)
        {
            IntersectionPolyline polyline = CheckTypeAndCast<IntersectionPolyline>(geometry);
            return Lenght.CompareTo(polyline.Lenght);
        }
    }
}