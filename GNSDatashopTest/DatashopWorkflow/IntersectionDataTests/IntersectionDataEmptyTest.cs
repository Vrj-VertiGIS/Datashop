using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEOCOM.GNSD.DatashopWorkflow.IntersectionData;
using NUnit.Framework;

namespace GNSDatashopTest.DatashopWorkflow.IntersectionDataTests
{
    [TestFixture]
    public class IntersectionDataEmptyTest
    {
        private IntersectionData _intersectionData;

        [SetUp]
        public void SetUp()
        {
            _intersectionData = new IntersectionData();
        }

        [Test]
        public void GetNameOfBiggestIntersectionTest()
        {
            string nameOfBiggestIntersection = _intersectionData.GetNameOfBiggestIntersection();
            Assert.IsEmpty(nameOfBiggestIntersection);
        }

        [Test]
        public void GetAllElementsNameSortedTest()
        {
            string allElementsNameSorted = _intersectionData.GetAllElementsNameSorted("---");
            Assert.IsEmpty(allElementsNameSorted);
        }
    }
}
