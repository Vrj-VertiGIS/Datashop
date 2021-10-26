using System;
using System.Collections.Generic;
using System.Reflection;
using GEOCOM.Common.Logging;
using GEOCOM.GNSDatashop.Model;
using NHibernate;
using NHibernate.Criterion;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
	public class PlotDefinitionStore 
	{
		// logging instance
        private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        #region IGNSDatastorePlotdefinition Members

        /// <summary>
		/// Return a collection of plot definitions. Filter by mediumCode if 
		/// not given as null (mediumCode != null).
		/// </summary>
		/// <param name="mediumCode">code of medium to filter for - or null</param>
		/// <returns>Collection of Plotdefinitions</returns>
		public ICollection<Plotdefinition> LoadForMedium(int? mediumCode)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				ICriteria crit = session.CreateCriteria(typeof(Plotdefinition));
				if (mediumCode != null)
                    crit.Add(Restrictions.Eq("PlotdefinitionKey.MediumCode", mediumCode.Value));
				return crit.List<Plotdefinition>();
			}
		}

        /// <summary>
        /// Returns a definition by id
        /// </summary>
        /// <param name="key">The key of the plot definition</param>
        /// <returns>null if not found</returns>
        public Plotdefinition GetPlotdefinition(PlotdefinitionKey key)
        {
			using (ISession session = NHibernateHelper.OpenSession())
			{
                ICriteria crit = session.CreateCriteria(typeof(Plotdefinition));
                crit.Add(Restrictions.And(Restrictions.Eq("PlotdefinitionKey.MediumCode", key.MediumCode), Restrictions.Eq("PlotdefinitionKey.Template", key.Template)));
				var templates = crit.List<Plotdefinition>();

				if (templates.Count == 0) return null;
				return templates[0];
            }
        }

        public bool Add(Plotdefinition definition)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Save(definition);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Adding a new plotdefinition failed", ex);
                    return false;
                }
            }
            return true;
        }

        public bool Update(Plotdefinition plotdefinition)
        {
            bool success = false;
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Update(plotdefinition);
                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Update plotdefinition failed", ex);
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a plotdefintion from db
        /// </summary>
        /// <param name="plotdefinition">The plotdefinition</param>
        /// <returns>true if no error, no templateName found resuls as true</returns>
        public bool Delete(Plotdefinition plotdefinition)
        {
            bool success = false;
            using (ISession session = NHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Delete(plotdefinition);
                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _log.Error("Delete plotdefinition failed", ex);
                }
            }
            return success;
        }

        #endregion
    }
}
