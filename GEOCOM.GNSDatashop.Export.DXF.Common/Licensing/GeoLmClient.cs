using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Licensing
{
	public class GeoLmClient : DllBase, IDisposable
	{
		#region wrapped functions prototypes
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TCheckOut(	[MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
										[MarshalAs(UnmanagedType.LPStr)] String psVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TCheckOutExtra(	[MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
											[MarshalAs(UnmanagedType.LPStr)] String psVersion,
											[MarshalAs(UnmanagedType.LPStr)] String psLicenseOrigin);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TCheckAvailability([MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
												[MarshalAs(UnmanagedType.LPStr)] String psVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TCheckIn(	[MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
										[MarshalAs(UnmanagedType.LPStr)] String psVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TErr_Info([MarshalAs(UnmanagedType.LPStr)] String psErrorMsg);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TSet_FlexLmFiles([MarshalAs(UnmanagedType.LPStr)] String psFiles);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TGet_FlexLmFiles([MarshalAs(UnmanagedType.LPStr)] String psFiles);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TSet_AutoAbort([MarshalAs(UnmanagedType.I4)] int bTerminate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TGet_AutoAbort([MarshalAs(UnmanagedType.I4)] ref int bTerminate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TGet_LicenseOrigin([MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
																					[MarshalAs(UnmanagedType.LPStr)] String psVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TTestLicense([MarshalAs(UnmanagedType.LPStr)] String psFeatureName,
																		 [MarshalAs(UnmanagedType.LPStr)] String psVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TBrowseUsedLicensesEx([MarshalAs(UnmanagedType.BStr)] ref String bstrFeature,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrLicType,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrVersion,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrLicCode,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrExpirationDate,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrLockToNode,
												   [MarshalAs(UnmanagedType.BStr)] ref String bstrServers,
												   [MarshalAs(UnmanagedType.I4)]   ref int    countLicenses,
												   [MarshalAs(UnmanagedType.U4)]       uint   bStart);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TBrowseLicenseSourceByIndexEx([MarshalAs(UnmanagedType.BStr)] ref String bstrFeature,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrLicType,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrVersion,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrLicCode,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrExpirationDate,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrLockToNode,
														   [MarshalAs(UnmanagedType.BStr)] ref String bstrServers,
														   [MarshalAs(UnmanagedType.I4)]   ref int countLicenses,
														   [MarshalAs(UnmanagedType.I4)]   int iLicNo,
														   [MarshalAs(UnmanagedType.BStr)] String bstrLicSourceName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TBrowseLicenseSourceEx([MarshalAs(UnmanagedType.BStr)] ref String bstrFeature,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrLicType,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrVersion,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrLicCode,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrExpirationDate,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrLockToNode,
													[MarshalAs(UnmanagedType.BStr)] ref String bstrServers,
													[MarshalAs(UnmanagedType.I4)]   ref int countLicenses,
													[MarshalAs(UnmanagedType.BStr)] String bstrLicSourceName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int TGet_CountLicenses();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TGetFLEXid([MarshalAs(UnmanagedType.BStr)] ref String bstrFLEXId,
																		[MarshalAs(UnmanagedType.I4)] int iIDType);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TIsBookedByProcess([MarshalAs(UnmanagedType.BStr)] String bstrFeatureName,
																						[MarshalAs(UnmanagedType.BStr)] String bstrFeatureVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TGetUsersOfFeature([MarshalAs(UnmanagedType.BStr)] String bstrFeatureName,
																						[MarshalAs(UnmanagedType.BStr)] ref String bstrUserList,
																						[MarshalAs(UnmanagedType.I4)] ref uint iTotalNumberOfLicenses);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TBrowseTrustedStorage([MarshalAs(UnmanagedType.BStr)] ref String Fulfillments);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TBrowseTrustedStorageLS([MarshalAs(UnmanagedType.BStr)] ref String fulfillments);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TSetActivationServerParameters([MarshalAs(UnmanagedType.BStr)] String commType,
																												[MarshalAs(UnmanagedType.BStr)] String commServer,
																												[MarshalAs(UnmanagedType.BStr)] String proxyServer,
																												[MarshalAs(UnmanagedType.U4)]   uint proxyPort,
																												[MarshalAs(UnmanagedType.BStr)] String proxyUserId,
																												[MarshalAs(UnmanagedType.BStr)] String proxyPassword);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TShutdown();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TBorrowFromServer([MarshalAs(UnmanagedType.BStr)] String productID,
		                                      [MarshalAs(UnmanagedType.BStr)] String serverName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TReturnBorrowedToServer([MarshalAs(UnmanagedType.BStr)] String fulfillmentId,
		                                             [MarshalAs(UnmanagedType.BStr)] String serverName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TActivateWithServer([MarshalAs(UnmanagedType.BStr)] String entitlementId,
			        							 [MarshalAs(UnmanagedType.BStr)] String expirationDate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineActivationProcessXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponseFromServer,
																															ref bool isConfigOnly);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineActivationIssueXMLRequest([MarshalAs(UnmanagedType.BStr)] String entitlementId,
																													 [MarshalAs(UnmanagedType.BStr)] String expirationDate,
																													 [MarshalAs(UnmanagedType.BStr)] ref String xmlRequestToServer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TReturnToServer([MarshalAs(UnmanagedType.BStr)] String fulfillmentId);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineReturnProcessXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponseFromServer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineReturnIssueXMLRequest([MarshalAs(UnmanagedType.BStr)] String fulfillmentId,
																											 [MarshalAs(UnmanagedType.BStr)] ref String xmlRequestToServer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TRepairWithServer([MarshalAs(UnmanagedType.BStr)] String fulfillmentId);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineRepairProcessXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponseFromServer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineRepairIssueXMLRequest([MarshalAs(UnmanagedType.BStr)] String fulfillmentId,
																											 [MarshalAs(UnmanagedType.BStr)] ref String xmlRequestToServer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate uint TGetActivationLastErrorCode();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void TGetActivationLastErrorString([MarshalAs(UnmanagedType.BStr)] ref String errorString);


		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TActivateLSWithServer([MarshalAs(UnmanagedType.BStr)] String entitlementId,
																							[MarshalAs(UnmanagedType.BStr)] String expirationDate);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineActivationProcessLSXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponse,
																															 [MarshalAs(UnmanagedType.Bool)] ref bool isConfigurationOnly);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineActivationIssueLSXMLRequest([MarshalAs(UnmanagedType.BStr)] String entitlementId,
																														[MarshalAs(UnmanagedType.BStr)] String expirationDate,
																														[MarshalAs(UnmanagedType.BStr)] ref String xmlRequest);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TReturnLSToServer([MarshalAs(UnmanagedType.BStr)] String fulfillmentId);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineReturnProcessLSXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponse);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineReturnIssueLSXMLRequest([MarshalAs(UnmanagedType.BStr)] String fulfillmentId,
																												[MarshalAs(UnmanagedType.BStr)] ref String xmlRequest);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TRepairLSWithServer([MarshalAs(UnmanagedType.BStr)] String fulfillmentId);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineRepairProcessLSXMLResponse([MarshalAs(UnmanagedType.BStr)] String xmlResponse);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate bool TOfflineRepairIssueLSXMLRequest([MarshalAs(UnmanagedType.BStr)] String fulfillmentId,
																												[MarshalAs(UnmanagedType.BStr)] ref String xmlRequest);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate String TGetBorrowUntilDate();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate	bool TIsBorrowingEnabled([MarshalAs(UnmanagedType.BStr)] String serverName);

		#endregion

		#region Wrapped functions exports

		// Accessor properties
		public TCheckOut CheckOut
		{ get { return (TCheckOut)base.GetProc<TCheckOut>(1); } }

		public TCheckOutExtra CheckOutExtra
		{ get { return (TCheckOutExtra)base.GetProc<TCheckOutExtra>(111); } }
		
		public TCheckAvailability CheckAvailability
		{ get { return (TCheckAvailability)base.GetProc<TCheckAvailability>(25); } }

		public TCheckIn CheckIn
		{ get { return (TCheckIn)base.GetProc<TCheckIn>(2); } }

		public TErr_Info Err_Info
		{ get { return (TErr_Info)base.GetProc<TErr_Info>(3); } }

		public TSet_FlexLmFiles Set_FlexLmFiles
		{ get { return (TSet_FlexLmFiles)base.GetProc<TSet_FlexLmFiles>(8); } }

		public TGet_FlexLmFiles Get_FlexLmFiles
		{ get { return (TGet_FlexLmFiles)base.GetProc<TGet_FlexLmFiles>(9); } }

		public TSet_AutoAbort Set_AutoAbort
		{ get { return (TSet_AutoAbort)base.GetProc<TSet_AutoAbort>(12); } }

		public TGet_AutoAbort Get_AutoAbort
		{ get { return (TGet_AutoAbort)base.GetProc<TGet_AutoAbort>(13); } }

		public TGet_LicenseOrigin Get_LicenseOrigin
		{ get { return (TGet_LicenseOrigin)base.GetProc<TGet_LicenseOrigin>(14); } }

		public TTestLicense TestLicense
		{ get { return (TTestLicense)base.GetProc<TTestLicense>(15); } }

		public TBrowseUsedLicensesEx BrowseUsedLicenses
		{ get { return (TBrowseUsedLicensesEx)base.GetProc<TBrowseUsedLicensesEx>(161); } }

		public TBrowseLicenseSourceByIndexEx BrowseLicenseSourceByIndex
		{ get { return (TBrowseLicenseSourceByIndexEx)base.GetProc<TBrowseLicenseSourceByIndexEx>(191); } }

		public TBrowseLicenseSourceEx BrowseLicenseSource
		{ get { return (TBrowseLicenseSourceEx)base.GetProc<TBrowseLicenseSourceEx>(181); } }

		public TGet_CountLicenses Get_CountLicenses
		{ get { return (TGet_CountLicenses)base.GetProc<TGet_CountLicenses>(17); } }

		public TGetFLEXid GetFLEXid
		{ get { return (TGetFLEXid)base.GetProc<TGetFLEXid>(20); } }

		public TIsBookedByProcess IsBookedByProcess
		{ get { return (TIsBookedByProcess)base.GetProc<TIsBookedByProcess>(21); } }

		public TGetUsersOfFeature GetUsersOfFeature
		{ get { return (TGetUsersOfFeature)base.GetProc<TGetUsersOfFeature>(22); } }

		public TShutdown Shutdown
		{get{ return (TShutdown) base.GetProc<TShutdown>(24); }}

		public TBorrowFromServer BorrowFromServer
		{ get { return (TBorrowFromServer) base.GetProc<TBorrowFromServer>("BorrowFromServer"); } }

		public TReturnBorrowedToServer ReturnBorrowedToServer
		{ get { return (TReturnBorrowedToServer) base.GetProc<TReturnBorrowedToServer>("ReturnBorrowedToServer"); } }

		public TSetActivationServerParameters SetActivationServerParameters
		{ get { return (TSetActivationServerParameters)base.GetProc<TSetActivationServerParameters>("SetActivationServerParameters"); } }

		public TGetActivationLastErrorCode GetActivationLastErrorCode
		{ get { return (TGetActivationLastErrorCode)base.GetProc<TGetActivationLastErrorCode>("GetActivationLastErrorCode"); } }

		public TGetActivationLastErrorString GetActivationLastErrorString
		{ get { return (TGetActivationLastErrorString)base.GetProc<TGetActivationLastErrorString>("GetActivationLastErrorString"); } }

		public TGetBorrowUntilDate GetBorrowUntilDate
		{ get { return (TGetBorrowUntilDate)base.GetProc<TGetBorrowUntilDate>("GetBorrowUntilDate"); } }

		public TIsBorrowingEnabled IsBorrowingEnabled
		{ get { return ((TIsBorrowingEnabled)base.GetProc<TIsBorrowingEnabled>("IsBorrowingEnabled")); } }

		/// <summary>
		/// Format a datetime value according to requirements given be FlexNet licensing (FEATURE/INCREMENT lines)
		/// </summary>
		/// <param name="expDate">Expiration date (time portion not processed)</param>
		/// <returns>Date according to FlexNet (US english, dd-MMM-yyyy)</returns>
		private String ToFNPExpirationDate(DateTime expDate)
		{
			return expDate.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("en-US"));
		}

		#endregion

		#region singleton "pattern" implementation - load only once within the entire process

		private static GeoLmClient _instance = null;
		private static LicenseAdminRunMode _currentRunMode;

		/// <summary>
		/// Get reference to GeoLmClient and load native dll if required.
		/// </summary>
		/// <param name="desiredRunMode">runmode (client/server)</param>
		/// <returns>reference to singleton object</returns>
		public static GeoLmClient Instance(LicenseAdminRunMode desiredRunMode)
		{
			if (_instance == null)
			{
				_instance = new GeoLmClient(desiredRunMode);
				_currentRunMode = desiredRunMode;
				return _instance;
			}

            if (desiredRunMode == _currentRunMode)
                return _instance;
            else
                throw new Exception(string.Format("Run mode change not allowed: from {0} to {1}", _currentRunMode.ToString(), desiredRunMode.ToString()));
		}

		#endregion

		#region construction/destruction

		/// <summary>
		/// Attach GeoLmClient.dll and implicitely make it's exported symbols available 
		/// </summary>
		protected GeoLmClient(LicenseAdminRunMode runMode)
		{
			var  dllName = (runMode.Equals(LicenseAdminRunMode.Server)) ? "GeoLmClientSvr.dll" : "GeoLmClient.dll";

            // A word regarding bitness of the library (32/64 bit) to be loaded:
            // The libraries are kept in subfolder GEOCOM\ of the standard $(CommonProgramFiles) resp. $(CommonProgramW6432)
            // shell folder. This automatically leads to loading of the suitable library, as System.Environment.GetFolderPath()
            // returns the respective folder according to application bitness
            var shellFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

            var pathGeoLmClient = (!shellFolder.EndsWith("\\"))
                ? shellFolder + "\\GEOCOM\\" + dllName
                : shellFolder + "GEOCOM\\" + dllName;

			if (File.Exists(pathGeoLmClient))
				base.Load(pathGeoLmClient);
			else
			{
				throw new FileNotFoundException(String.Format("Library {0} not found in {1}.\nPlease have your installation repaired.", dllName, shellFolder));
			}
		}

		#endregion

		#region wrapped accessors with special logic

		/// <summary>
		/// Activate licenses with activation server - for a limited time
		/// Transparently handle server- as well as client trusted storage activation
		/// </summary>
		/// <param name="entitlementId">ID of the entitlement</param>
		/// <param name="expirationDate">Date (including) until which activation should be valid.(</param>
		/// <returns>true if activation successful. false <see cref="GetActivationLastErrorCode"/></returns>
		public bool ActivateWithServer(String entitlementId, String expirationDate)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TActivateWithServer)base.GetProc<TActivateWithServer>("ActivateWithServer"))(entitlementId, expirationDate);
			else
				return ((TActivateLSWithServer)base.GetProc<TActivateLSWithServer>("ActivateLSWithServer"))(entitlementId, expirationDate);
		}

		/// <summary>
		/// Activate licenses with activation server - for a limited time
		/// </summary>
		/// <param name="entitlementId">ID of the entitlement</param>
		/// <param name="expirationDate">Date (including) until which activation should be valid.(</param>
		/// <returns>true if activation successful. false <see cref="GetActivationLastErrorCode"/></returns>
		public bool ActivateWithServer(String entitlementId, DateTime expirationDate)
		{
			return ActivateWithServer(entitlementId, ToFNPExpirationDate(expirationDate));
		}

		/// <summary>
		/// Activate licenses with activation server for an unlimited time.
		/// </summary>
		/// <param name="entitlementId">ID of the entitlement</param>
		/// <returns>true if activation successful. false <see cref="GetActivationLastErrorCode"/></returns>
		public bool ActivateWithServer(String entitlementId)
		{
			return ActivateWithServer(entitlementId, "permanent");
		}

		/// <summary>
		/// Process the activation (authorization) response we received (i.e. by eMail) from the activation server.
		/// Handle both, the license server- and the client trusted storage case.
		/// </summary>
		/// <param name="xmlResponseFromServer">activation (or deny) response received from activation server</param>
		/// <param name="isConfigOnly">Is the response from the server only a config response and do we have to resend again?</param>
		/// <returns>true if activation successful. false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineActivationProcessXMLResponse(String xmlResponseFromServer, ref bool isConfigOnly)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineActivationProcessXMLResponse)(base.GetProc<TOfflineActivationProcessXMLResponse>("OfflineActivationProcessXMLResponse")))(xmlResponseFromServer, ref isConfigOnly);
			else
				return ((TOfflineActivationProcessLSXMLResponse)(base.GetProc<TOfflineActivationProcessLSXMLResponse>("OfflineActivationProcessLSXMLResponse")))(xmlResponseFromServer, ref isConfigOnly);
		}

		/// <summary>
		/// Create XML document to issue an activation (authorization) request to the activation server offline (by eMail etc.).
		/// Handle both, the license server- and the client trusted storage case.
		/// </summary>
		/// <param name="entitlementId">id of the entitlement to be authorized</param>
		/// <param name="expirationDate">date of expiration desired for the authorization</param>
		/// <param name="xmlRequestToServer">request xml to be sent to the activation server.</param>
		/// <returns>true if activation successful. false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineActivationIssueXMLRequest(String entitlementId, 	String expirationDate, ref String xmlRequestToServer)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineActivationIssueXMLRequest)(base.GetProc<TOfflineActivationIssueXMLRequest>("OfflineActivationIssueXMLRequest")))(entitlementId, expirationDate, ref xmlRequestToServer);
			else
				return ((TOfflineActivationIssueLSXMLRequest)(base.GetProc<TOfflineActivationIssueLSXMLRequest>("OfflineActivationIssueLSXMLRequest")))(entitlementId, expirationDate, ref xmlRequestToServer);
		}

		/// <summary>
		/// Create activation xml request (to be sent to an activation server i.e. by email or similar)
		/// for an activation valid until a given date.
		/// </summary>
		/// <param name="entitlementId">ID of the entitlement</param>
		/// <param name="expirationDate">Date (including) until which activation should be valid.(</param>
		/// <param name="xmlRequest">Resulting xml request</param>
		/// <returns>true if request created successfully, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineActivationIssueXMLRequest(String entitlementId, DateTime expirationDate, ref String xmlRequest)
		{
			return OfflineActivationIssueXMLRequest(entitlementId, ToFNPExpirationDate(expirationDate), ref xmlRequest);
		}

		/// <summary>
		/// Create activation xml request (to be sent to an activation server i.e. by email or similar)
		/// for an unlimited amount of time.
		/// </summary>
		/// <param name="entitlementId">ID of the entitlement</param>
		/// <param name="xmlRequest">Resulting xml request</param>
		/// <returns>true if request created successfully, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineActivationIssueXMLRequest(String entitlementId, ref String xmlRequest)
		{
			return OfflineActivationIssueXMLRequest(entitlementId, ToFNPExpirationDate(DateTime.MaxValue), ref xmlRequest);
		}

		/// <summary>
		/// Return a fulfillment to the activation server.
		/// Handle both, the license server- and the client trusted storage case
		/// </summary>
		/// <param name="fulfillmentId">Id of the fulfillment to return</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool ReturnToServer(String fulfillmentId)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TReturnToServer)(base.GetProc<TReturnToServer>("ReturnToServer")))(fulfillmentId);
			else
				return ((TReturnLSToServer)(base.GetProc<TReturnLSToServer>("ReturnLSToServer")))(fulfillmentId);
		}

		/// <summary>
		/// Process response for a return which we received as an XML file (i.e. by eMail) 
		/// Handle license server- as well as client side trusted storage
		/// </summary>
		/// <param name="xmlResponseFromServer">the response received from the activation server</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineReturnProcessXMLResponse(String xmlResponseFromServer)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineReturnProcessXMLResponse)(base.GetProc<TOfflineReturnProcessXMLResponse>("OfflineReturnProcessXMLResponse")))(xmlResponseFromServer);
			else
				return ((TOfflineReturnProcessLSXMLResponse)(base.GetProc<TOfflineReturnProcessLSXMLResponse>("OfflineReturnProcessLSXMLResponse")))(xmlResponseFromServer);
		}

		/// <summary>
		/// Create a request for a return which we will then send (i.e. via eMail) to the activation server for processing.
		/// Handle license server- as well as client side trusted storage
		/// </summary>
		/// <param name="fulfillmentId">id of the fulfillment to be returned</param>
		/// <param name="xmlRequestToServer">request xml to be stored as a file and then sent to the activation server</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineReturnIssueXMLRequest(String fulfillmentId, ref String xmlRequestToServer)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineReturnIssueXMLRequest)(base.GetProc<TOfflineReturnIssueXMLRequest>("OfflineReturnIssueXMLRequest")))(fulfillmentId, ref xmlRequestToServer);
			else
				return ((TOfflineReturnIssueLSXMLRequest)(base.GetProc<TOfflineReturnIssueLSXMLRequest>("OfflineReturnIssueLSXMLRequest")))(fulfillmentId, ref xmlRequestToServer);
		}

		/// <summary>
		/// Try repair of "broken" trusted storage segment with the activation server online.
		/// Handle license server- as well as client side trusted storage.
		/// </summary>
		/// <param name="fulfillmentId">id of the fulfillment contained within the broken ts segment</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool RepairWithServer(String fulfillmentId)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TRepairWithServer)(base.GetProc<TRepairWithServer>("RepairWithServer")))(fulfillmentId);
			else
				return ((TRepairLSWithServer)(base.GetProc<TRepairLSWithServer>("RepairLSWithServer")))(fulfillmentId);
		}

		/// <summary>
		/// Process response for a repair which we received as an XML file (i.e. by eMail) from the activation server
		/// Handle license server- as well as client side trusted storage
		/// </summary>
		/// <param name="xmlResponseFromServer">the response received from the activation server</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool OfflineRepairProcessXMLResponse(String xmlResponseFromServer)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineRepairProcessXMLResponse)(base.GetProc<TOfflineRepairProcessXMLResponse>("OfflineRepairProcessXMLResponse")))(xmlResponseFromServer);
			else
				return ((TOfflineRepairProcessLSXMLResponse)(base.GetProc<TOfflineRepairProcessLSXMLResponse>("OfflineRepairProcessLSXMLResponse")))(xmlResponseFromServer);
		}

		/// <summary>
		/// Create a request for a repair which we will then send (i.e. via eMail) to the activation server for processing.
		/// Handle license server- as well as client side trusted storage
		/// </summary>
		/// <param name="fulfillmentId">id of the fulfillment of the TS segment to be repaired</param>
		/// <param name="xmlRequestToServer">request xml to be stored as a file and then sent to the activation server</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		bool OfflineRepairIssueXMLRequest(String fulfillmentId, ref String xmlRequestToServer)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TOfflineRepairIssueXMLRequest)(base.GetProc<TOfflineRepairIssueXMLRequest>("OfflineRepairIssueXMLRequest")))(fulfillmentId, ref xmlRequestToServer);
			else
				return ((TOfflineRepairIssueLSXMLRequest)(base.GetProc<TOfflineRepairIssueLSXMLRequest>("OfflineRepairIssueLSXMLRequest")))(fulfillmentId, ref xmlRequestToServer);
		}

		/// <summary>
		/// Browse trusted storage for available licenses.
		/// Handle both, the license server- and client trusted storage case.
		/// </summary>
		/// <param name="Fulfillments">Fulfillments given in xml format</param>
		/// <returns>true if successful, false <see cref="GetActivationLastErrorCode"/></returns>
		public bool BrowseTrustedStorage(ref String Fulfillments)
		{
			if (_currentRunMode == LicenseAdminRunMode.Client)
				return ((TBrowseTrustedStorage)(base.GetProc<TBrowseTrustedStorage>("BrowseTrustedStorage")))(ref Fulfillments);
			else
				return ((TBrowseTrustedStorageLS)(base.GetProc<TBrowseTrustedStorageLS>("BrowseTrustedStorageLS")))(ref Fulfillments);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Clean up and release unmanaged ressources
		/// </summary>
		public override void Dispose()
		{
			if (!_disposed)
			{
				Shutdown();
				base.Dispose();
				_disposed = true;
			}
		}

		#endregion
	}

    public enum LicenseAdminRunMode
    {
        Client,
        Server
    }

}