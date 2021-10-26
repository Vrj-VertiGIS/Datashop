using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Display;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    /// <summary>
    /// wrapping the esri cancel tracker so having a save and straightforward access to it.
    /// </summary>
    public class CancelTrackerDummy : ITrackCancel
    {
        private ITrackCancel _esriCancelTracker = null;

        private bool _cancelling = false;

        public event EventHandler<EventArgs> OnCancel = null;
        public event EventHandler<CancelTrackerDummyBeforeCancelEventEventArgs> OnBeforeCancel = null;

        /// <summary>
        /// Placeholder for server use
        /// </summary>
        public CancelTrackerDummy() { }

        /// <summary>
        /// Create a cancel tracker "dummy" for use in ArcMap context
        /// </summary>
        public CancelTrackerDummy(ITrackCancel nativeCancelTracker)
        {
            _esriCancelTracker = nativeCancelTracker;
        }

        public ITrackCancel NativeObject => _esriCancelTracker;

        private void RaiseCancelEvent()
        {
            if (!_cancelling)
            {
                var cancelEventArgs = new CancelTrackerDummyBeforeCancelEventEventArgs() { AbortCancelRequest = false };
                OnBeforeCancel?.Invoke(this, cancelEventArgs);

                _cancelling = !cancelEventArgs.AbortCancelRequest;

                if (_cancelling)
                    OnCancel?.Invoke(this, new EventArgs());
            }
        }

        #region ITrackCancel implementation

        public void Cancel()
        {
            if ((null != _esriCancelTracker) && (!_cancelling))
                _esriCancelTracker.Cancel();        // Cancel() Events will be reaised upon the next call too continue()
            else
                RaiseCancelEvent();
        }

        public void Reset()
        {
            if (null != _esriCancelTracker)
                _esriCancelTracker.Reset();

            _cancelling = false;
        }

        public bool Continue()
        {
            if (!_cancelling)
                // Override esri's cancel-tracker behaviour:
                // All subsequent calls to Continue() will return false
                // if the user has cancelled - this opposite to arcObjects,
                // where only the call just following the cancel event returns false.
                if (!(_esriCancelTracker?.Continue() ?? true))
                    RaiseCancelEvent(); // Will eventually set _cancelling to true

            return !_cancelling;
        }

        public void StartTimer(int hWnd, int Milliseconds)
        {
            if (null != _esriCancelTracker)
                _esriCancelTracker.StartTimer(hWnd, Milliseconds);
        }

        public void StopTimer()
        {
            if (null != _esriCancelTracker)
                _esriCancelTracker.StopTimer();
        }

        public int CheckTime
        {
            get => _esriCancelTracker?.CheckTime ?? 1000;
            set
            {
                if (null != _esriCancelTracker)
                    _esriCancelTracker.CheckTime = value;
            }
        }

        public IProgressor Progressor
        {
            get => _esriCancelTracker?.Progressor;
            set
            {
                if (null != _esriCancelTracker)
                    _esriCancelTracker.Progressor = value;
            }
        }

        public bool ProcessMessages
        {
            get => _esriCancelTracker?.ProcessMessages ?? true;
            set
            {
                if (null != _esriCancelTracker)
                    _esriCancelTracker.ProcessMessages = value;
            }
        }

        public bool TimerFired =>
            _esriCancelTracker?.TimerFired ?? false;

        public bool CancelOnClick
        {
            get => _esriCancelTracker?.CancelOnClick ?? false;
            set
            {
                if (null != _esriCancelTracker)
                    _esriCancelTracker.CancelOnClick = value;
            }
        }
        public bool CancelOnKeyPress
        {
            get => _esriCancelTracker?.CancelOnKeyPress ?? false;
            set
            {
                if (null != _esriCancelTracker)
                    _esriCancelTracker.CancelOnKeyPress = value;
            }
        }

        #endregion
    }
}
