using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEOCOM.GNSD.DatashopWorkflow.IntersectionData
{
    /// <summary>
    /// Holds intersection data in a collection and provides methods to query them.
    /// </summary>
    public class IntersectionData
    {
        private readonly List<IntersectionGeometry> _intersectionGeometries = new List<IntersectionGeometry>();

        /// <summary>
        /// Adds intersection geometry to an inner collection.
        /// </summary>
        /// <param name="geometry"></param>
        public void Add(IntersectionGeometry geometry)
        {
            IntersectionGeometry foundIntersectionGeometry = _intersectionGeometries.Find(x => x.Name == geometry.Name);
            if (foundIntersectionGeometry != null)
                foundIntersectionGeometry.Add(geometry);
            else
                _intersectionGeometries.Add(geometry);
        }

        /// <summary>
        /// Return a name of the biggest intesection in the collection.
        /// </summary>
        /// <returns></returns>
        public string GetNameOfBiggestIntersection()
        {
            IntersectionGeometry biggestIntersection = _intersectionGeometries.Max();
            return biggestIntersection != null ? biggestIntersection.Name : string.Empty;
        }


        /// <summary>
        /// Returns all elements names sorted by thier significance and delimited by the passed separator.  
        /// </summary>
        public string GetAllElementsNameSorted(string separator)
        {
            StringBuilder intersectionStringBuilder = new StringBuilder();
            bool useDefaultSeparator = string.IsNullOrEmpty(separator) || separator.Equals("%(nl)");
            if (useDefaultSeparator)
                separator = Environment.NewLine;

            // Sort descending
            _intersectionGeometries.Sort((firstItem, nextItem) => nextItem.CompareTo(firstItem));

            _intersectionGeometries.ForEach(g => intersectionStringBuilder.AppendFormat("{0}{1}", g.Name, separator));

            bool isStringBuilderEmpty = intersectionStringBuilder.Length == 0;
            if (isStringBuilderEmpty)
                return string.Empty;

            int stringLenghtWithoutLastSeparator = intersectionStringBuilder.Length - separator.Length;
            string intersectionString = intersectionStringBuilder.ToString(0, stringLenghtWithoutLastSeparator);


            return intersectionString;
        }
    }
}
