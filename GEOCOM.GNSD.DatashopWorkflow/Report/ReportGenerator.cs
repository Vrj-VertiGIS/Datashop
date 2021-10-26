using System;
using System.Collections.Generic;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;

namespace GEOCOM.GNSD.DatashopWorkflow.Report
{
    public class ReportGenerator
    {
        public void Generate(string targetFile, string reportFile, IDictionary<string, string> variables)
        {
            using (StiReport report = new StiReport())
            {
                report.Load(reportFile);

                foreach (StiComponent comp in report.GetComponents())
                {
                    if (comp is StiText)
                    {
                        StiText text = comp as StiText;

                        text.Text.Value = Utils.Utils.ReplaceVars(text.Text.Value, variables);

                        text.Text.Value = Utils.Utils.ReplaceVars(text.Text.Value, variables, "<", ">");

                    }
                }

                report.Render(false);

                StiPdfExportSettings settings = new StiPdfExportSettings();
                settings.ImageQuality = 0.85f;
                settings.ImageCompressionMethod = StiPdfImageCompressionMethod.Jpeg;
                settings.ImageFormat = StiImageFormat.Color;

                report.ExportDocument(StiExportFormat.Pdf, targetFile, settings);
            }
        }
    }
}
