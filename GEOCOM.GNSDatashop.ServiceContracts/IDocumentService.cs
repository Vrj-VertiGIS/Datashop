using System;
using System.ServiceModel;
using GEOCOM.GNSDatashop.Model.Documents;

namespace GEOCOM.GNSDatashop.ServiceContracts
{
    /// <summary>
    /// Interface definition for service that uploads and downloads files
    /// </summary>
    [ServiceContract]
    public interface IDocumentService
    {
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [OperationContract]
        DocumentDownload GetDocument(DocumentRequest request);

        /// <summary>
        /// Gets the document. Used in DatashopAddon Java and GWT project
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [OperationContract]
        DocumentDownload GetDocumentDatashopAddon(DocumentRequestDatashopAddon request);

        /// <summary>
        /// Saves the document.
        /// </summary>
        /// <param name="upload">The upload.</param>
        [OperationContract]
        void SaveDocument(DocumentUpload upload);
    }
}