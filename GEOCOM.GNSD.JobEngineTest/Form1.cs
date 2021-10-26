using System;
using System.Windows.Forms;
using GEOCOM.GNSD.Common.JobFactory;
using GEOCOM.GNSD.Common.Model;
using GEOCOM.GNSD.DBStore.Container.JobData;
using GEOCOM.GNSD.DBStore.DbAccess;
using GNSPlotExtensionConst = GEOCOM.GNSD.Common.Model.GNSPlotExtensionConst;

namespace GEOCOM.GNSD.JobEngineTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MapExtent[] mapExtents = new[]
                                         {
                                             new MapExtent()
                                                 {CenterX = 683289, CenterY = 683289, Rotation = 0, Scale = 50, PlotTemplate = "a3_quer"},
                                             new MapExtent()
                                                 {CenterX = 683289, CenterY = 683289, Rotation = 45, Scale = 500, PlotTemplate = "a3_hoch"},
                                             new MapExtent()
                                                 {CenterX = 683289, CenterY = 683289, Rotation = 90, Scale = 1000, PlotTemplate = "a4_quer"},
                                         };
            ExportModel model = new PdfExportJobFactory().CreateJob( mapExtents);
            textBox1.Text = CreateJob(model.ToXml(), GNSPlotExtensionConst.PROGID).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExportPerimeter perimeter = new ExportPerimeter();
            perimeter.PointCollection = new ExportPerimeter.CoordinatePair[4];
            perimeter.PointCollection[0] = new ExportPerimeter.CoordinatePair { X = 600000, Y = 200100 };
            perimeter.PointCollection[1] = new ExportPerimeter.CoordinatePair { X = 600100, Y = 200100 };
            perimeter.PointCollection[2] = new ExportPerimeter.CoordinatePair { X = 600100, Y = 200000 };
            perimeter.PointCollection[3] = new ExportPerimeter.CoordinatePair { X = 600000, Y = 200000 };

            TdeExportModel model = new TdeExportModel();
            model.Perimeters = new[] { perimeter, perimeter };
            model.ProfileGuid = "123456000";
            model.OutputFormat = GEOCOM.GNSD.Common.Model.OutputFormat.pgdb; // GEOCOM.TDE.Enumerations.OutputFormat.pgdb;
            textBox2.Text = CreateJob(model.ToXml(), TdeExtensionConst.PROGID).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //TransferredFiles transferredFiles = new TransferredFiles();
            //transferredFiles.Files = new string[] { "test.xml" };
            //transferredFiles.TransferId = new Guid("{B5A3D62E-85C5-4a2c-BD6E-3C4739B1974D}");
            //textBox3.Text = CreateJob(transferredFiles.ToString(), TransferredFiles.PROGID).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AdminJobModel adminJobModel = new AdminJobModel();
            adminJobModel.Action = AdminJobConst.UPDATEPLOTTEMPLATES;
            textBox4.Text = CreateJob(adminJobModel.ToXml(), AdminJobConst.PROGID).ToString();
        }

        private long CreateJob(string definition, string processorClassId)
        {
            JobStore jobStore = new JobStore();
            Job job = new Job();
            job.Definition = definition;
            job.Step = 0;
            job.State = 0;
            job.ProcessorClassId = processorClassId;
            job.ReasonId = 1;
            job.UserId = 1;
            job.PeriodBeginDate = DateTime.Now;
            job.PeriodEndDate = DateTime.Now.AddDays(30);

            return jobStore.Add(job);
        }
    }
}
