using System;
using ESRI.ArcGIS.Geometry;

namespace GEOCOM.GNSD.DatashopWorkflow.GeoDataBase
{
    public class ExtentData 
    {
        #region class member variables

        #endregion

        #region Construction/dextruction

        public ExtentData()
        {
        }

        public ExtentData(int? ownerId, int? extentObjectId, IGeometry extentShape, string extentDescription)
        {
            this.OwnerId = ownerId;
            this.ExtentObjectId = extentObjectId;
            this.ExtentShape = extentShape;
            this.ExtentDescription = extentDescription;
        }

        public ExtentData(object ownerId, object extentObjectId, object extentShape, object extentDescription)
        {
            if ((ownerId != null) && (ownerId != DBNull.Value) && (ownerId is int))
            {
                this.OwnerId = (int)ownerId;
            }

            if ((extentObjectId != null) && (extentObjectId != DBNull.Value) && (extentObjectId is int))
            {
                this.ExtentObjectId = (int)extentObjectId;
            }

            if ((extentShape != null) && (extentShape != DBNull.Value) && ((extentShape as IGeometry) != null))
            {
                this.ExtentShape = extentShape as IGeometry;
            }

            if ((extentDescription != null) && (extentDescription != DBNull.Value) && (extentDescription is string))
            {
                this.ExtentDescription = (string)extentDescription;
            }
        }

        #endregion

        #region accessors

        public int? OwnerId { get; private set; }

        public int? ExtentObjectId { get; private set; }

        public IGeometry ExtentShape { get; private set; }

        public string ExtentDescription { get; private set; }

        #endregion

        #region public methods

        /// <summary>
        /// This comparer is used to sort a list of DataOwnerExtentInfo according
        /// to the user id 
        /// </summary>
        /// <param name="x">
        /// Compare DataOwnerExtentInfo ...
        /// </param>
        /// <param name="y">
        /// Against DataOwnerExtentInfo
        /// </param>
        /// <returns>
        /// 0 if both equal, -1 if y &gt; x, 1 if x &gt; y
        /// </returns>
        public static int CompareByUserIds(ExtentData x, ExtentData y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x.OwnerId == null)
            {
                if (y.OwnerId == null)
                {
                    return 0;
                }
                
                return -1;
            }

            if (y.OwnerId == null)
            {
                return 1;
            }
            
            return x.OwnerId.Value.CompareTo(y.OwnerId.Value);
        }

        #endregion
    }
}
