using System;
using GEOCOM.GNSD.DatashopWorkflow.IntersectionData;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWorkflow.IntersectionDataTests
{
    [TestFixture]
    public class IntersectionGeometryTest
    {
        private IntersectionArea _area;
        private const string AreaName = "intersection_area_name";
        private const double AreaValue = 99.9;

        private IntersectionPolyline _polyline;
        private const string PolylineName = "intersection_polyline_name";
        private const double PolylineLenght = 999.9;

        private IntersectionPoint _point;
        private const string PointName = "intersection_point_name";


        [SetUp]
        public void SetUp()
        {
            _area = new IntersectionArea(AreaName, AreaValue);
            _polyline = new IntersectionPolyline(PolylineName, PolylineLenght);
            _point = new IntersectionPoint(PointName);
        }

        [Test]
        public void TypeCheckAndCastTest()
        {
            Assert.Throws<ArgumentException>(() => _area.CompareTo(_polyline));
            Assert.Throws<ArgumentException>(() => _area.Add(_polyline));
         
            Assert.Throws<ArgumentException>(() => _polyline.CompareTo(_area));
            Assert.Throws<ArgumentException>(() => _polyline.Add(_area));
         
            Assert.Throws<ArgumentException>(() => _point.CompareTo(_area));
            Assert.Throws<ArgumentException>(() => _point.Add(_area));
        }

        #region Area tests

        [Test]
        public void IntersectionAreaTest()
        {
       
            Assert.AreEqual(AreaName, _area.Name);
            Assert.AreEqual(AreaValue, _area.Area);
        }

        [Test]
        public void AreaAddTest()
        {
            double areaValueToAdd = 10;
            string areaName = "testArea";
            IntersectionArea testArea = new IntersectionArea(areaName, areaValueToAdd);
            testArea.Add(_area);

            Assert.AreEqual(areaValueToAdd + AreaValue, testArea.Area) ;
            Assert.AreEqual(areaName, testArea.Name);

            //test that the added area remains unchanged
            Assert.AreEqual(AreaName, _area.Name);
            Assert.AreEqual(AreaValue, _area.Area);
        }

        [Test]
        public void AreaCompareTest()
        {
            double areaValueToAdd = 10;
            string areaName = "testArea";
            IntersectionArea testArea = new IntersectionArea(areaName, areaValueToAdd);
            int compareTo = _area.CompareTo(testArea);
            Assert.AreEqual(1, compareTo);
        }

        #endregion

        #region Polyline tests

        [Test]
        public void IntersectionPolylineTest()
        {
            Assert.AreEqual(PolylineName, _polyline.Name);
            Assert.AreEqual(PolylineLenght, _polyline.Lenght);
        }

        [Test]
        public void PolylineAddTest()
        {
            double lenghtValueToAdd = 100;
            string lineName = "testLine";
            IntersectionPolyline testPolyline = new IntersectionPolyline(lineName, lenghtValueToAdd);
            testPolyline.Add(_polyline);

            Assert.AreEqual(PolylineLenght + lenghtValueToAdd, testPolyline.Lenght);
            Assert.AreEqual(lineName,testPolyline.Name);

            //test that the added polyline remains unchanged
            Assert.AreEqual(PolylineName, _polyline.Name);
            Assert.AreEqual(PolylineLenght, _polyline.Lenght);
        }

        [Test]
        public void PolylineCompareTest()
        {
            double lenghtValueToAdd = 100;
            string lineName = "testLine";
            IntersectionPolyline testPolyline = new IntersectionPolyline(lineName, lenghtValueToAdd);
            int compareTo = _polyline.CompareTo(testPolyline);
            Assert.AreEqual(1, compareTo);
        }

        #endregion

        #region Point tests

        [Test]
        public void IntersectionPointTest()
        {
            Assert.AreEqual(PointName, _point.Name);
        }

        [Test]
        public void PointAddTest()
        {
            string testPointName = "testPoint";
            IntersectionPoint testPoint = new IntersectionPoint(testPointName);
            testPoint.Add(_point);
            Assert.AreEqual(testPointName, testPoint.Name);
            Assert.AreEqual(PointName, _point.Name);
        }

        [Test]
        public void PointCompareTest()
        {
            string testPointName = "testPoint";
            IntersectionPoint testPoint = new IntersectionPoint(testPointName);
            int compareTo = _point.CompareTo(testPoint);
            Assert.AreEqual(0, compareTo);
        }

        #endregion
    }
}
