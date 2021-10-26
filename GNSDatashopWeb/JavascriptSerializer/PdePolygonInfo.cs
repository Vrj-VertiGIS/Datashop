using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEOCOM.GNSD.Web.JavascriptSerializer
{
    public class PdePolygonInfo
    {
        public PdePolygonRing[] Rings;
    }
    public class PdePolygonRing
    {
        public PdePolygonPoint[] Points;
    }
    public class PdePolygonPoint
    {
        public double[] Point;
    }
}
