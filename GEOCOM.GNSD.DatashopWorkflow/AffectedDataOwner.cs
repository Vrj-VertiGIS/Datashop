using System.Collections.Generic;
using GEOCOM.GNSD.DatashopWorkflow.GeoDataBase;

namespace GEOCOM.GNSD.DatashopWorkflow
{
    using System.Linq;

    public class AffectedDataOwner
    {
        private readonly DataOwner _owner;
        private readonly List<ExtentData> _extents;

        public AffectedDataOwner(DataOwner owner, List<ExtentData> extents)
        {
            _owner = owner;
            _extents = extents;
        }

        public DataOwner Owner
        {
            get
            {
                return _owner;
            }
        }

        public List<ExtentData> Extents
        {
            get
            {
                return _extents;
            }
        }

        public bool HasToBeNotified
        {
            get { return (Extents.Count > 0) && (Owner.EMail != string.Empty); }
        }

        public IList<string> GetAllExtentDescriptions()
        {
            return this.Extents.Select(doei => doei.ExtentDescription).ToList();
        }
    }
}
