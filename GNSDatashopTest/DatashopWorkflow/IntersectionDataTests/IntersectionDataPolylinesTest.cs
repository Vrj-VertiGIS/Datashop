using GEOCOM.GNSD.DatashopWorkflow.IntersectionData;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWorkflow.IntersectionDataTests
{
    [TestFixture]
    public class IntersectionDataPolylinesTest
    {
        private string _name1;
        private double _lenth1;
        private IntersectionPolyline _polyline1;
        
        private string _name2;
        private double _lenth2;
        private IntersectionPolyline _polyline2;

        private string _name3;
        private double _lenth3;
        private IntersectionPolyline _polyline3;
        private IntersectionData _intersectionData;

        [SetUp]
        public void SetUp()
        {
            _name1 = "polyline1";
            _lenth1 = 10;
            _polyline1 = new IntersectionPolyline(_name1, _lenth1);

            _name2 = "polyline2";
            _lenth2 = 20;
            _polyline2 = new IntersectionPolyline(_name2, _lenth2);

            _name3 = "polyline3";
            _lenth3 = 30;
            _polyline3 = new IntersectionPolyline(_name3, _lenth3);

            _intersectionData = new IntersectionData();
            _intersectionData.Add(_polyline1);
            _intersectionData.Add(_polyline2);
            _intersectionData.Add(_polyline3);
            _intersectionData.Add(_polyline3);

        }

        [Test]
        public void IntersectionDataGetNameOfBiggestIntersectionTest()
        {
            string biggestIntersectionName = _intersectionData.GetNameOfBiggestIntersection();
            Assert.AreEqual(_name3,biggestIntersectionName);
        }

        [Test]
        public void IntersectionDataAllTest()
        {
            string allElementsNameSorted = _intersectionData.GetAllElementsNameSorted("---");
            string expected = string.Format("{0}---{1}---{2}", _polyline3.Name, _polyline2.Name, _polyline1.Name);
            Assert.AreEqual(expected,allElementsNameSorted);
        }
    }
}
