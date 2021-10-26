namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public interface IArcPointInfo : IPointInfo
    {
        double Bulge { get; }
    }

    public class ArcPointInfo : PointInfo
    {
        private double _bulge;

        public ArcPointInfo(double x, double y, double bulge)
            : base(x, y)
        {
            Flags = PointInfoFlags.ArcPoint;

            _bulge = bulge;
        }

        public ArcPointInfo(double x, double y, double z, double bulge)
            : base(x, y, z)
        {
            Flags = PointInfoFlags.ArcPoint;

            _bulge = bulge;
        }

        public double Bulge
        {
            get
            {
                return _bulge;
            }
        }
    }
}
