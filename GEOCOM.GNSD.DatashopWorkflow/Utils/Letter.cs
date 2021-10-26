using System;
using System.IO;
using System.Text.RegularExpressions;
using GEOCOM.GNSD.DatashopWorkflow.Report;

namespace GEOCOM.GNSD.DatashopWorkflow.Utils
{
    public class Letter
    {
        /// <summary>
        /// Creates the letter PDF.
        /// </summary>
        /// <param name="DataItem">The data item.</param>
        public static void CreateLetterPdf(DatashopWorkflowDataItem DataItem)
        {
            string path = Path.GetDirectoryName(DataItem.JobOutput);
            string reportTemplate = DataItem.LetterTemplate.File;

            if (File.Exists(reportTemplate))
            {
                // create report instance
                ReportGenerator report = new ReportGenerator();

                string fileName;
                if (DataItem.LetterTemplate.TargetFile == null)
                {
                    fileName = "Letter.pdf";  // Default
                }
                else
                {
                    fileName = Utils.ReplaceVars(DataItem.LetterTemplate.TargetFile, DataItem.Variables);
                    CheckFilename(fileName);
                }

                string fullQualifiedFileName = Path.Combine(path, fileName);

                // generate letter as pdf
                report.Generate(fullQualifiedFileName, reportTemplate, DataItem.Variables);
            }
            else
            {
                DatashopWorkflowDataItem.Logger.WarnFormat("Letter template file {0} not found!", reportTemplate);
            }
        }

        /// <summary>
        /// Checks the filename.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void CheckFilename(string fileName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(fileName))
            {
                throw new Exception(string.Format("Invalid character in fileName={0}", fileName));
            }

            string extension = Path.GetExtension(fileName);
            if ((fileName != null) && (extension != null))
            {
                if (!extension.Equals(".pdf"))
                {
                    DatashopWorkflowDataItem.Logger.DebugFormat("Changing file extension to 'pdf'. Please adjust fileNameTemplate in configuration file to match the file extension *.pdf.");
                    Path.ChangeExtension(fileName, "pdf");
                }
            }
        }
    }
}
