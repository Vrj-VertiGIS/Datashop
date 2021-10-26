using System.Collections.Generic;
using System.ServiceModel;
using GEOCOM.GNSDatashop.Model.AddressSearch;

namespace GEOCOM.GNSDatashop.ServiceContracts
{
    /// <summary>
    /// Service contract for the addres search service
    /// </summary>
    [ServiceContract]
    public interface IAddressSearchService
    {
        /// <summary>
        /// Gets the search definitions.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GeoFind GetSearchDefinitions();

        /// <summary>
        /// Performs the geo search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        [OperationContract]
        GeoSearch PerformGeoSearch(GeoSearch search);

        /// <summary>
        /// Performs the result extent search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <returns></returns>
        [OperationContract]
        List<ResultExtent> PerformResultExtentSearch(GeoSearch search);
    }
}