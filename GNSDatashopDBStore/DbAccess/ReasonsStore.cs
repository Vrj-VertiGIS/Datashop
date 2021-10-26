using System.Collections.Generic;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model;
using NHibernate;
using NHibernate.Criterion;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
	public class ReasonsStore
	{
		// logging instance
		private static IMsg _log = new Msg(typeof(PlotDefinitionStore));

		/// <summary>
		/// Load reasons (Reason) table for look-up
		/// </summary>
		/// <returns>Collection of reason (Reason) entries</returns>
		public virtual ICollection<Reason> Load()
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				ICriteria crit = session.CreateCriteria(typeof(Reason));
				return crit.List<Reason>();
			}
		}

        public void Add(int id, string description)
        {
 			using (ISession session = NHibernateHelper.OpenSession())
			{
                Reason reason = new Reason();
			    reason.ReasonId = id;
			    reason.Description = description;
			    session.Save(reason);
                session.Flush();
			}
        }

        public Reason Get(long id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return (Reason)session.CreateCriteria(typeof(Reason))
                                        .Add(Restrictions.Eq("ReasonId", id))
                                        .UniqueResult();
            }
        }
	}
}
