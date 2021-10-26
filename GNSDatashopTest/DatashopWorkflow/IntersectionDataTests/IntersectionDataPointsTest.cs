using GEOCOM.GNSD.DatashopWorkflow.IntersectionData;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWorkflow.IntersectionDataTests
{
    [TestFixture]
    class IntersectionDataPointsTest
    {
        private string _name1;
        private IntersectionPoint _point1;

        private string _name2;
        private IntersectionPoint _point2;

        private string _name3;
        private IntersectionPoint _point3;

        private IntersectionData _intersectionData;

        [SetUp]
        public void SetUp()
        {
            _name1 = "point1";
            _point1 = new IntersectionPoint(_name1);

            _name2 = "point2";
            _point2 = new IntersectionPoint(_name2);

            _name3 = "point3";
            _point3 = new IntersectionPoint(_name3);

            _intersectionData = new IntersectionData();
            _intersectionData.Add(_point1);
            _intersectionData.Add(_point2);
            _intersectionData.Add(_point3);
            _intersectionData.Add(_point3);
        }

       [Test]
        public void IntersectionDataAllTest()
        {
            string allElementsNameSorted = _intersectionData.GetAllElementsNameSorted("---");
            string expected = string.Format("{0}---{1}---{2}", _point3.Name, _point2.Name, _point1.Name);
            Assert.AreEqual(expected.Length, allElementsNameSorted.Length);
            Assert.True(allElementsNameSorted.Contains(_point1.Name));
            Assert.True(allElementsNameSorted.Contains(_point2.Name));
            Assert.True(allElementsNameSorted.Contains(_point3.Name));
            Assert.True(allElementsNameSorted.Contains("---"));
        }
    }
}
