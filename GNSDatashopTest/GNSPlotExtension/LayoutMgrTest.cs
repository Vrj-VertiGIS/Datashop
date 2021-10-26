using ESRI.ArcGIS;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSD.PlotExtension.Layout;
using GNSDatashopTest.TestUtils;
using NUnit.Framework;

namespace GNSDatashopTest.GNSPlotExtension
{
    /// <summary>
    /// Tests for the LayoutMgr class
    /// </summary>
    [TestFixture]
	[Ignore("Requires ArcGIS license and thus cannot run on the build server.")]
    public class LayoutMgrTest
    {
        /// <summary>
        /// Initialises the license.
        /// </summary>
        [SetUp]
        public void InitialiseLicense()
        {
           RuntimeManager.BindLicense(ProductCode.Server);
        }

        /// <summary>
        /// Tests that the CalcPlotExtend method should return an IPolygon
        /// </summary>
        [Test]
        public void CalcPlotExtendShouldReturnIPolygon()
        {
            var calcPlotExtendMethod = typeof (LayoutMgr).GetStaticMethod("CalcPlotExtend");

            var centerX = 0.0;
            var centerY = 0.0;
            var scale = 0.0;
            var rotation = 0.0;
            var frameWidth = 100.0;
            var frameHeight = 100.0;

            var actual = calcPlotExtendMethod.Invoke(null, new object[] { centerX, centerY, scale, rotation, frameWidth, frameHeight });

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is IPolygon);
        }
    }
}