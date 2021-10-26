using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class ResultExtent
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public ResultExtent()
        {
        }

        public ResultExtent(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}