using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Logging;
using GEOCOM.GNSD.DatashopWorkflow.Utils;
using GEOCOM.GNSD.DBStore.Archive;
using System;
using System.Diagnostics;
using System.Linq;

namespace GEOCOM.GNSD.JobEngine
{
    public class Program
    {

        static IMsg log = new Msg(typeof(Program));

        //NOTE: Dxf export will not work without this attribute. Don't ask me why. 
        //TODO: find out why it was missing in the first place
        //http://msdn.microsoft.com/en-us/library/system.stathreadattribute.aspx
        [STAThread]
        private static void Main(string[] args)
        {

            long jobId;
            if (args.Length > 0 && long.TryParse(args[0], out jobId))
            {
                DatashopLogInitializer.Initialize(jobId.ToString());

                var startTime = DateTime.Now;
                var proc = Process.GetCurrentProcess();
                log.Info($"Starting a job processing for jobId {jobId}, PID = {proc.Id}, at {DateTime.Now:f}");
                JobProcessor jp = new JobProcessor(jobId);
                bool force = args.Length >= 2 && args[1] == "-force";
                jp.ExecuteJob(jobId, force);
                log.Info($"Finished a job processing {jobId} after {DateTime.Now - startTime:c}");

                Utils.RelaseAllWorkspaces();
                Suicider.Commit();
            }
            else if (args.Length == 2 && args[0] == "-archive" && DateTime.TryParse(args[1], out var createdBefore))
            {
                DatashopLogInitializer.Initialize("archive");

                log.Info($"Archiving jobs created before {createdBefore}.");
                var jobArchiver = new JobArchiver();
                jobArchiver.Archive(createdBefore);

                log.Info("Finished jobs archiving.");
            }
            else if (args.Length >= 2 && args[0] == "-archive")
            {
                DatashopLogInitializer.Initialize("archive");

                var jobIds = args.Skip(1).Select(id => long.TryParse(id, out var _jobid) ?_jobid : -1).Where(_jobId => _jobId >= 0)
                    .ToArray();
                log.Info($"Archiving jobs by ids {string.Join(", ",jobIds)}.");
                var jobArchiver = new JobArchiver();
                jobArchiver.Archive(jobIds);
                log.Info("Finished jobs archiving.");
            }
            else
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("GEOCOM.GNSD.JobEngine.exe JobID // starts a job processing by its job id");
                Console.WriteLine("GEOCOM.GNSD.JobEngine.exe JobID -force // restarts a job's workflow and starts the job processing by its job id");
                Console.WriteLine("GEOCOM.GNSD.JobEngine.exe -archive \"YYYY-MM-DD [hh:mm:ss]\" // archives jobs created before the specified date and optionally time and older");
                Console.WriteLine("GEOCOM.GNSD.JobEngine.exe -archive jobid jobid jobid ... // archives jobs by their job ids");
            }
        }
    }
}
