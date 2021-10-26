using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.Licensing
{
    public class LicenseHolder : IDisposable
    {
        private License _feature;

        private GeoLmClient _lmClient = null;
        private object _lmClientMutex = new object();

        private bool _canChackAvailability = true;  // Try - and if not possible - set this to false;

        /// <summary>
        /// Some - temporary - constants
        /// </summary>
        /// 
        public const string Feature_DXF_Export = @"GEONIS_DXFEXPORT";

        public License.LicenseStatus Status => (null != _feature) ? _feature.Status : License.LicenseStatus.untested;

        public License Feature => _feature;

        // Status changed available<>unavailable, Valid<>invalid
        public event EventHandler<LicenseStatusChangedEventArgs> OnStatusChanged;                   

        public LicenseHolder(string featureName, string version)
        {
            _feature = new License(featureName, version);
            _feature.Status = License.LicenseStatus.untested;    
        }

        /// <summary>
        /// Check License status but do not checkout the license (no
        /// change in availability to others).
        /// </summary>
        /// <returns>Status (validity) of the license</returns>
        /// 
        public License.LicenseStatus CheckAvailability()
        {
            var status = (0 != CheckAvailability(_feature.FeatureName, _feature.Version))
                ? License.LicenseStatus.available
                : License.LicenseStatus.unavailable;

            SetLicenseStatus(status);

            return status;
        }

        /// <summary>
        /// Check License status. If license available, check it out
        /// for use by our process. Subsequent calls to this method
        /// will not check out a second license.
        /// </summary>
        /// <returns>Status (validity) of the license</returns>
        public License.LicenseStatus CheckOut()
        {
            var status = (0 != CheckOut(_feature.FeatureName, _feature.Version))
                ? License.LicenseStatus.checkedOut
                : License.LicenseStatus.invalid;

            SetLicenseStatus(status);

            return status;
        }

        /// <summary>
        /// Return (check-in) license after use. 
        /// </summary>
        /// <returns></returns>
        public License.LicenseStatus Return()
        {
            CheckIn(_feature.FeatureName, _feature.Version);

            SetLicenseStatus(License.LicenseStatus.available);  // Must be available since otheerwise we couldn't have checked out this.

            return License.LicenseStatus.available;
        }

        private int CheckAvailability(string featureName, string featureVersion)
        {
            if (_canChackAvailability)
            {
                lock (_lmClientMutex)
                {
                    try
                    {
                        return LicenseClient.CheckAvailability(featureName, featureVersion);
                    }
                    catch (NotImplementedException)
                    {
                        // assuming feature initially present - will lead to activated
                        // command button but starting the tool will fail due to the
                        // real license check (checkout)
                        _canChackAvailability = false;  // remember for later;
                        return 1;
                    }
                }
            }
            else
                return 1;
        }

        private int CheckOut(string featureName, string featureVersion)
        {
            lock (_lmClientMutex)
                return LicenseClient.CheckOut(featureName, featureVersion);
        }

        private void CheckIn(string featureName, string featureVersion)
        {
            lock (_lmClientMutex)
                LicenseClient.CheckIn(_feature.FeatureName, _feature.Version);
        }

        public void Dispose()
        {
            if (null != _lmClient)
                lock (_lmClientMutex)
                {
                    (_lmClient as IDisposable).Dispose();

                    _lmClient = null;
                }
        }

        private GeoLmClient LicenseClient
        {
            get
            {
                if (null == _lmClient)
                    _lmClient = GeoLmClient.Instance(LicenseAdminRunMode.Client);
                return _lmClient;
            }
        }

        private void FireLicenseStatusChangedEvent(License.LicenseStatus newStatus, EventHandler<LicenseStatusChangedEventArgs> theEvent)
        {
            _feature.Status = newStatus;
            var eventargs = new LicenseStatusChangedEventArgs(_feature);

            theEvent?.Invoke(this, eventargs);

            OnStatusChanged?.Invoke(this, eventargs);
        }

        private void FireLicenseStatusChangedEvent(License.LicenseStatus newStatus)
        {
            var eventargs = new LicenseStatusChangedEventArgs(_feature);

            OnStatusChanged?.Invoke(this, eventargs);
        }

        private void SetLicenseStatus(License.LicenseStatus newStatus)
        {
            _feature.Status = newStatus;

            FireLicenseStatusChangedEvent(newStatus);
            /*
            switch (_feature.Status)
            {
                case License.LicenseStatus.untested:
                    if (newStatus == License.LicenseStatus.invalid)
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToInvalid);
                    else if (newStatus == License.LicenseStatus.unavailable)
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToUnavailable);
                    else if (newStatus == License.LicenseStatus.available)
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToAvailable);
                    else if (newStatus == License.LicenseStatus.checkedOut)
                        FireLicenseStatusChangedEvent(newStatus, onStatusChangedToCheckedOut);
                    else
                        ThrowInvalidStatusChange(newStatus);
                    break;
                case License.LicenseStatus.unavailable:
                    if (newStatus == License.LicenseStatus.invalid)
                        ;   // Tried to check out - unsuccesful - No status change - feature still unavailable
                    else if (newStatus == License.LicenseStatus.available)
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToAvailable);
                    else if (newStatus == License.LicenseStatus.checkedOut)
                        FireLicenseStatusChangedEvent(newStatus, onStatusChangedToCheckedOut);
                    else
                        ThrowInvalidStatusChange(newStatus);
                    break;
                case License.LicenseStatus.available:
                    if (newStatus == License.LicenseStatus.invalid)
                        ;  // Tried to check out - unsuccesful - No status change - feature still available but license cannot be used (count or similar)
                    else if (newStatus == License.LicenseStatus.unavailable)
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToUnavailable);
                    else if (newStatus == License.LicenseStatus.available)
                        ; // Retested - license still available - no change and thus no event
                    else if (newStatus == License.LicenseStatus.checkedOut)
                        FireLicenseStatusChangedEvent(newStatus, onStatusChangedToCheckedOut);
                    else
                        ThrowInvalidStatusChange(newStatus);
                    break;
                case License.LicenseStatus.checkedOut:
                    if (newStatus == License.LicenseStatus.invalid)
                        // License suddenly became invalid - license server loss, outdated,...
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToInvalid);
                    else if (newStatus == License.LicenseStatus.unavailable)
                        // License suddenly became invalid - license server loss, outdated,...
                        FireLicenseStatusChangedEvent(newStatus, OnStatusChangedToUnavailable);
                    else if (newStatus == License.LicenseStatus.available)
                        ; // License checked out - must be available tough - no change of the current status
                    else if (newStatus == License.LicenseStatus.checkedOut)
                        ; // Retested - license still checked out and ready for use.
                    else
                        ThrowInvalidStatusChange(newStatus);

                    break;
                case License.LicenseStatus.invalid:
                    ThrowInvalidStatusChange(newStatus); // Inpossible - license can either be unavailable, available or chedked out - but neve will be set to invalid
                    break;
                default:
                    ThrowInvalidStatusChange(newStatus); // Don't know why we landed heere
                    break;
            }
            */
        }
    }
}
