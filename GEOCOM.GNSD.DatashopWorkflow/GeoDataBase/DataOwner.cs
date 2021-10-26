namespace GEOCOM.GNSD.DatashopWorkflow.GeoDataBase
{
    public class DataOwner
    {
        #region private member vars
        private int? _ownerId;
        private string _email;
        private string _description;
        #endregion

        #region construction/destruction
        public DataOwner(int? ownerId, string email, string description)
        {
            _ownerId = ownerId;
            _email = email;
            _description = description;
        }
        #endregion

        #region accessors

        public int? OwnerId
        {
            get
            {
                return _ownerId;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string EMail
        {
            get
            {
                return _email;
            }
        }

        #endregion
    }
}
