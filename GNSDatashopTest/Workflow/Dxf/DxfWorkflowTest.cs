using System;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GEONIS.GeonisCentralObjects;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.DatashopWorkflow.Dxf;
using GNSDatashopTest.TestUtils;
using Moq;
using NUnit.Framework;

namespace GNSDatashopTest.Workflow.Dxf
{
	[TestFixture]
	public class DxfWorkflowTest
	{
		//TODO: despite the amount of tests we are nowhere near 100% code coverage because the workflow model is far too tightly coupled 
		//TODO: to be able to write meaningful unit tests where we can reliably just test the workflow classes

		#region Private Methods

		private void AssertException<TException>(MethodInfo method, DxfWorkflow exportWorkflow, params object[] parameters) where TException : Exception
		{
			try
			{
				method.Invoke(exportWorkflow, parameters);
			}
			catch (Exception ex)
			{
				Assert.IsNotNull(ex.InnerException);
				Assert.IsInstanceOf<TException>(ex.InnerException);
			}
		}

		#endregion

		#region ShouldGenerateTemporaryLyrFilePath Tests

		[Test]
		public void GenerateTemporaryLyrFilePathShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("GenerateTemporaryLyrFilePath");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "");
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, " ");
		}

		//[Test]
		public void ShouldGenerateTemporaryLyrFilePath()
		{
			//TODO: find out how we can properly test the actual methods that have a dependency to the dataitems
			//var exportWorkflow = new DxfWorkflow();

			//var exportWorkflowType = exportWorkflow.GetType();

			//var dataItem = new WorkflowDataItemBase
			//                   {
			//                       Job = new Job
			//                                 {
			//                                     JobId = 1
			//                                 }
			//                   };

			//var dataItemProperty = exportWorkflowType.GetField("DataItem", BindingFlags.Instance);

			//Assert.IsNotNull(dataItemProperty);

			//dataItemProperty.SetValue(dataItem, dataItem);

			//var method = exportWorkflowType.GetInstanceMethod("GenerateTemporaryLyrFilePath");

			//var result = method.Invoke(exportWorkflow, new[] { @"c:\datashop\lyfile.lyr" });

			//Assert.IsNotNull(result);
			//Assert.IsInstanceOf<string>(result);
			//Assert.AreEqual("", result.ToString());
		}

		#endregion

		#region ShouldValidateLyrFile Tests

		[Test]
		public void ValidateOriginalLyrFileShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("ValidateOriginalLyrFile");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "");
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, " ");
		}

		[Test]
		public void ValidateOriginalLyrFileShouldThrowInvalidOperationException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("ValidateOriginalLyrFile");

			this.AssertException<InvalidOperationException>(method, exportWorkflow, @"c:\some\arbitrary\path\to\file.lyr");
		}

		[Test]
		public void ValidateTemporaryLyrFileShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("ValidateTemporaryLyrFile");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "");
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, " ");
		}

		[Test]
		public void ValidateTemporaryLyrFileShouldThrowInvalidOperationException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("ValidateTemporaryLyrFile");

			this.AssertException<InvalidOperationException>(method, exportWorkflow, @"c:\some\arbitrary\path\to\file.lyr");
		}

		#endregion

		#region PrepareLyrFileTests

		[Test]
		public void PrepareLyrFileShouldThrowArgumentNullExceptionWithNewPath()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("PrepareLyrFile");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "", null);
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, " ", null);
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "ArbitraryValue", null);
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "ArbitraryValue", "");
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, "ArbitraryValue", " ");
		}

		#endregion

		#region CreateExportProtocol Tests

		[Test]
		[Ignore(@"This test instantiate the class GCErrors which a COM wrapper of a class that might not be register.")]
		public void CreateExportProtocolShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("CreateExportProtocol");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null, null });
			// TODO Remove the GCErrors COM wrapper
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new GCErrors(), null });
		}

		[Test]
		public void CreateExportProtocolFromRawShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("CreateExportProtocolFromRaw");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null });
		}

		#endregion

		#region PerformExport Tests

		[Test]
		[STAThread]
		public void PerformExportShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("PerformExport");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null, null, null, null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, null, null, null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), null, null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), "", null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), " ", null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), "ArbitraryValue", null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), "ArbitraryValue", "", null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), "ArbitraryValue", " ", null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { new Mock<IGeometry>().Object, new DxfExportInfo(), "ArbitraryValue", "ArbitraryValue", null });
		}

		#endregion

		#region ExportPerimeter Tests

		[Test]
		public void ExportPerimeterShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("ExportPerimeter");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, null, null, null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, new Mock<IGeometry>().Object, null, null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, new Mock<IGeometry>().Object, new DxfExportInfo(), null, null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, new Mock<IGeometry>().Object, new DxfExportInfo(), "", null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, new Mock<IGeometry>().Object, new DxfExportInfo(), " ", null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { 0, new Mock<IGeometry>().Object, new DxfExportInfo(), "ArbitraryValue", null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { -1, null, null, null, null });
		}

		#endregion

		#region CleanupExport Tests

		[Test]
		public void CleanupExportShouldThrowArgumentNullException()
		{
			var exportWorkflow = new DxfWorkflow();

			var exportWorkflowType = exportWorkflow.GetType();

			var method = exportWorkflowType.GetInstanceMethod("CleanupExport");

			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { null });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { "" });
			this.AssertException<GEOCOM.Common.Exceptions.AssertionException>(method, exportWorkflow, new object[] { " " });
		}

		#endregion
	}
}