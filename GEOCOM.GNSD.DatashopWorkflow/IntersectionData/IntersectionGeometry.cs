using System;

namespace GEOCOM.GNSD.DatashopWorkflow.IntersectionData
{
    /// <summary>
    /// Represents a result of an interesection.
    /// </summary>
    public abstract class IntersectionGeometry : IComparable<IntersectionGeometry>
    {
        /// <summary>
        /// String extracted from a geometry's columns.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds value of the passed geometry to value of this geometry.
        /// </summary>
        public abstract void Add(IntersectionGeometry geometry);

        public abstract int CompareTo(IntersectionGeometry geometry);

        /// <summary>
        /// Performs a type match check and cast. If the check fails, an ArgumentException exception is thrown;
        /// </summary>
        protected virtual TTarget CheckTypeAndCast<TTarget>(IntersectionGeometry geometryToCheck) where TTarget : class 
        {
            bool typesIncompatible = (geometryToCheck as TTarget) == null;
            if (typesIncompatible)
            {
                string message = string.Format("IntersectionGeometry mismatch. Expected {0}, but acquired {1}.", GetType(), geometryToCheck.GetType());
                throw new ArgumentException(message);
            }

            return geometryToCheck as TTarget;

        }
    }
}