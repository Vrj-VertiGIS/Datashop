using System;
using System.Collections.Generic;
using System.Text;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using GEOCOM.Common.Logging;

namespace GEOCOM.GNSD.DBStore
{
	[TestFixture]
	public class Test_GNSDatashopDBStore
	{
		[Test]
		public void GNSDUsers_add()
		{
			GNSDatastoreUser userStore = new GNSDatastoreUser();
			// add a new user
			GNSD_User u = new GNSD_User { Firstname = "test", Lastname = "case", Email = "test@geocom.ch" };
			Assert.IsFalse(userStore.Add(u));
			u = new GNSD_User { Firstname = "test", Lastname = "case", Email = DateTime.Now.GetHashCode().ToString() + "@geocom.ch" };
			Assert.IsTrue(userStore.Add(u));
		}

		[Test]
		public void GNSDJob_add()
		{
			GNSDatastoreJob jobStore = new GNSDatastoreJob();
			// add a new user

			// wif wrapper for xml added
			// generate the job Description
			JobDescriptionXML jobDescription = new JobDescriptionXML();
			jobDescription.CenterX = 615300;
			jobDescription.CenterY = 219350;
			jobDescription.Rotation = 0;
			jobDescription.Scale = 5000;

			GNSD_Job u = new GNSD_Job { Definition = "point=600000,200000;scale=5000", Purpose = "just for testing", UserID = 12 };
			u.Definition = jobDescription.ToString();

			Assert.AreNotEqual(jobStore.Add(u),-1);
		}

		[Test]
		public void GNSDJob_GetJobByID()
		{
			GNSDatastoreJob jobStore = new GNSDatastoreJob();
			// add a new user

			// wif wrapper for xml added
			// generate the job Description
			JobDescriptionXML jobDescription = new JobDescriptionXML();
			jobDescription.CenterX = 615300;
			jobDescription.CenterY = 219350;
			jobDescription.Rotation = 0;
			jobDescription.Scale = 5000;

			GNSD_Job u = new GNSD_Job { Definition = "point=600000,200000;scale=5000", Purpose = "just for testing", UserID = 12 };
			u.Definition = jobDescription.ToString();
			long jID = jobStore.Add(u);
			Assert.AreNotEqual(jID, -1);

			GNSD_Job u2 = jobStore.GetByID(jID);

		}


		[Test]
		public void GNSDJob_GetNewJobs()
		{
			GNSDatastoreJob jobStore = new GNSDatastoreJob();
			// add a new user
			GNSD_Job u = new GNSD_Job { Definition = "point=600000,200000;scale=5000", Purpose = "just for testing", UserID = 12 };
			ICollection<GNSD_Job> jobs = jobStore.GetNewJobs();
			Assert.Greater(jobs.Count, 0);
			//foreach (GNSD_Job j in jobs)
			//{
			//   System.Console.WriteLine("job userid: " + j.UserID);
			//}

		}

		[Test]
		public void GNSDJob_SetStatus()
		{
			GNSDatastoreJob jobStore = new GNSDatastoreJob();
			GNSD_Job u = new GNSD_Job {Definition = "point=600000,200000;scale=5000", Purpose = "just for testing", UserID = 12 };
			long jID = jobStore.Add(u);
			u.Status = GNSD_Job.JobStatus.FINISHED;
			Assert.IsTrue(jobStore.Update(u));
			Assert.AreEqual(u.Status, GNSD_Job.JobStatus.FINISHED);
		}

	}
}
