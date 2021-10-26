using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using netDxf;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    class PathPoint
    {
        private uint _pathType;
        private uint _pointType;
        private PointF _coordinates;
        public uint PathType
        {
            get { return _pathType; }
        }
        public uint PointType
        {
            get { return _pointType; }
        }
        public PointF Coordinates
        {
            get { return _coordinates; }
        }

        public Vector2 Vector
        {
            get { return new Vector2(_coordinates.X, _coordinates.Y); }
        }

        public PathPoint(GraphicsPath path, int pointIndex)
        {
            if (pointIndex < path.PointCount)
            {
                _coordinates = path.PathPoints[pointIndex];
                _pathType = path.PathTypes[pointIndex];
                _pointType = (_pathType & 0x7);
            }
            else
                throw new IndexOutOfRangeException("point index out of range while parsing path oints");
        }

    }
}
