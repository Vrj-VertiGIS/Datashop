using System.Collections.Generic;
using GEOCOM.GNSDatashop.Model;
using NHibernate;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    public class PlotSectionStore
    {
        public IList<PlotSection> GetAllSections()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {                
                var jobSections = session.CreateCriteria(typeof(PlotSection)).List<PlotSection>();

                if (jobSections.Count == 0) return new List<PlotSection>();

                return jobSections;
            }
        }
    }
}
