using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    [Flags]
    public enum PointInfoFlags
    {
        SimpleLinePoint = 1,
        ArcPoint = 2
    }

    public interface IPointInfo
    {
        double X { get; set; }
        double Y { get; set; }
        double? Z { get; set; }
        PointInfoFlags Flags { get; }
    }

    public class PointInfo : IPointInfo
    {

        public PointInfoFlags Flags { get; protected set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }

        public PointInfo(double x, double y)
        {
            Flags = PointInfoFlags.SimpleLinePoint;

            X = x;
            Y = y;
            Z = null;
        }

        public PointInfo(double x, double y, double z) 
            : this(x, y)
        {
            Z = z;
        }
    }
}
