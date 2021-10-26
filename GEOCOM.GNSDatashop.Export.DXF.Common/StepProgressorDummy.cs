using System;
using ESRI.ArcGIS.esriSystem;

using ESRI.ArcGIS.Framework;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public class StepProgressorDummy : IStepProgressor
    {
        private IStepProgressor _esriStepProgressor = null;

        private decimal _stepsPerPart = 1;      // Number of progressor steps per part (assuming we subdivide the entire range)

        private decimal _stepsPerCount = 1;     // Number of progressor steps per user item (i.e. row) to count

        private decimal _accumulatedSteps = 0;  // Accumulated steps (may be fractions of a step)

        private int _stepValue = 1;             // Handy shortcut for _esriStepProgressor.StepValue

        public StepProgressorDummy() { }

        public StepProgressorDummy(IStepProgressor stepProgressor)
            : this()
        {
            _esriStepProgressor = stepProgressor;
        }

        /// <summary>
        /// Create a step progressor wrapper (dummy) including it's native wrapped coclass step progressor
        /// </summary>
        /// <param name="application">ArcGIS application</param>
        /// <returns>Wtep progressor wrapper (dummy) object</returns>
        public static StepProgressorDummy FromApplication(IApplication application)
        {
            if (null == application) throw new ArgumentNullException("application");

            var sp = new StepProgressorDummy(application.StatusBar.ProgressBar)
            { MinRange = 0, MaxRange = 1000, StepValue = 1 };

            sp.nParts = 1;

            return sp;
        }

        /// <summary>
        /// Native COM interface of teh wrapped StepProgressor
        /// </summary>
        public IStepProgressor NativeObject => _esriStepProgressor;

        #region Application specific members

        /// <summary>
        /// Number of parts - i.e. files to be exportet (at once).
        /// </summary>
        public uint nParts
        {
            get => (uint)((MaxRange - MinRange) / _stepsPerPart);
            set
            {
                _stepsPerPart = (MaxRange - MinRange) / (decimal)value;
            }
        }

        /// <summary>
        /// The counts (user items - i.e. rows) currently assigned to a part (the current part)
        /// </summary>
        public uint PartSize
        {
            get => (uint)(_stepsPerPart / _stepsPerCount);
            set
            {
                _stepsPerCount = _stepsPerPart / value;
                StepValue = (_stepsPerCount >= 1) ? (int)_stepsPerCount : 1;
                // FitPositionToParts();
            }
        }

        /// <summary>
        /// Step the progressor one atomic item (progressor range / nParts / PartSize) 
        /// </summary>
        public void StepItem()
        {
            for (_accumulatedSteps += _stepsPerCount; (_stepValue <= _accumulatedSteps); _accumulatedSteps -= _stepValue)
                _esriStepProgressor.Step();
        }

        /// <summary>
        /// Step the progressor one entire part (progressor range / nParts)
        /// </summary>
        public void StepPart()
        {
            PartSize = 1;
            StepItem();
        }

        #endregion

        #region Quriks 

        public void ShowFullProgress()
        {
            if (null != _esriStepProgressor)
            {
                Position = MaxRange;
                System.Threading.Thread.Sleep(500); // Show this for a halve of a second
            }
        }
        #endregion

        #region IStepProgressor implementation
        public void Show()
        {
            if (null != _esriStepProgressor)
                _esriStepProgressor.Show();
        }

        public void Step()
        {
            if (null != _esriStepProgressor)
                _esriStepProgressor.Step();
        }

        public void Hide()
        {
            if (null != _esriStepProgressor)
                _esriStepProgressor.Hide();
        }

        public int OffsetPosition(int offsetValue)
        {
            if (null != _esriStepProgressor)
            {
                var offset = _esriStepProgressor.OffsetPosition(offsetValue);
                return offset;
            }
            else
                return 0;
        }

        public string Message
        {
            get => (null != _esriStepProgressor)
                ? _esriStepProgressor.Message
                : String.Empty;
            set
            {
                if (null != _esriStepProgressor)
                    _esriStepProgressor.Message = value;
            }
        }

        public int MinRange
        {
            get => (null != _esriStepProgressor)
                ? _esriStepProgressor.MinRange
                : int.MinValue;
            set
            {
                if (null != _esriStepProgressor)
                    _esriStepProgressor.MinRange = value;
            }
        }

        public int MaxRange
        {
            get => (null != _esriStepProgressor)
                ? _esriStepProgressor.MaxRange
                : int.MaxValue;
            set
            {
                if (null != _esriStepProgressor)
                    _esriStepProgressor.MaxRange = value;
            }
        }

        public int Position
        {
            get => (null != _esriStepProgressor)
                ? _esriStepProgressor.Position
                : int.MinValue;
            set
            {
                if (null != _esriStepProgressor)
                    _esriStepProgressor.Position = value;
            }
        }

        public int StepValue
        {
            get
            {
                // be shure to use the real step progressor's value heere
                // but also cache it for later direct access (shortcut)
                var sv = (null != _esriStepProgressor)
                    ? _esriStepProgressor.StepValue
                    : _stepValue;
                _stepValue = sv;
                return sv;
            }
            set
            {
                _stepValue = value;     // We want this shortcut for our processing (performance)
                if (null != _esriStepProgressor)
                    _esriStepProgressor.StepValue = _stepValue;
            }
        }
        #endregion

    }
}
