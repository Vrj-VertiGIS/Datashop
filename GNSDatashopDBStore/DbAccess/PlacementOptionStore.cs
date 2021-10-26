using System;
using System.Linq;
using System.Collections.Generic;
using GEOCOM.GNSDatashop.Model;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    /// <summary>
    /// Repository class for PlacementOptions
    /// </summary>
    public class PlacementOptionStore
    {
        /// <summary>
        /// Gets all entries from the DB
        /// </summary>
        /// <returns></returns>
        public List<PlacementOption> GetAll()
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    return session.CreateCriteria(typeof(PlacementOption))
                        .List<PlacementOption>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PlacementOptionStore.GetAll() failed", ex);
            }
        }
    }
}