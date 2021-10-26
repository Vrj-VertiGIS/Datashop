using System;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DatashopWorkflow.DataWorkflow;
using GEOCOM.GNSD.DatashopWorkflow.Utils;

namespace GEOCOM.GNSD.DatashopWorkflow.Pde
{
    public class PdeWorkflow : DataWorkflowBase
    {
        protected override void Process()
        {
            TdeExportModel tdeExportModel = DataItem.JobDescriptionModel as TdeExportModel;
            DataExtractor extractor = new DataExtractor(tdeExportModel);
            if (tdeExportModel != null)
            {
                for (int i = 0; i < tdeExportModel.Perimeters.Length; i++)
                {
                    string outputFileName = this.GetOutputFileName(i);
                    DatashopWorkflowDataItem.Logger.InfoFormat("Extracting data for perimeter={0}", i);
                    extractor.Extract(outputFileName);
                }

                DatashopWorkflowDataItem.Logger.InfoFormat("Creating letter PDF file");
                Letter.CreateLetterPdf(this.DataItem);
            }
            else
            {
                throw new Exception("Invalid export model.");
            }
        }
    }
}