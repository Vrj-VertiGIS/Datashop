using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Core.ServerControls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg">The MSG.</param>
    public delegate void ValidationFailedHandler(string msg);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate bool ValidationSkipConditionHandler();

    /// <summary>
    /// Base class for validated controls
    /// </summary>
    public abstract class ValidatedControl : UserControl
    {
        #region Public Properties

        /// <summary>
        /// Occurs when [validation failed].
        /// </summary>
        public event ValidationFailedHandler ValidationFailed;

        /// <summary>
        /// Gets or sets the validation failed text.
        /// </summary>
        /// <value>
        /// The validation failed text.
        /// </value>
        public string ValidationFailedText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ValidatedControl"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Gets or sets the validation skip condition.
        /// </summary>
        /// <value>
        /// The validation skip condition.
        /// </value>
        public ValidationSkipConditionHandler ValidationSkipCondition { get; set; }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>
        /// The validation group.
        /// </value>
        public virtual string ValidationGroup { get; set; }

        /// <summary>
        /// If set to DataItem it will favor the values from a DataItem before the values defined at the control definition
        /// </summary>
        public ValidatedControlDataBindingPriority DataBindingPriority { get; set; } 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatedControl"/> class.
        /// </summary>
        protected ValidatedControl()
        {
            DataBindingPriority = ValidatedControlDataBindingPriority.DataItem;
        } 

        #endregion


        #region Public MEthods

        /// <summary>
        /// Datas the bind.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        public virtual void DataBind(ValidatedControlDataItem dataItem)
        {
            if (DataBindingPriority == ValidatedControlDataBindingPriority.DataItem)
            {
                Visible = dataItem.Visible;
                Required = dataItem.Required;
            }
        } 

        #endregion

        #region Page lifecycle events

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AddValidatorToControls();
        }

        /// <summary>
        /// Raises the <see cref="E:Validate"/> event.
        /// </summary>
        /// <param name="e">The <see cref="GEOCOM.GNSD.Web.Core.ServerControls.ValidateEventArgs"/> instance containing the event data.</param>
        protected virtual void OnValidate(ValidateEventArgs e)
        {
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the validator to controls.
        /// </summary>
        private void AddValidatorToControls()
        {
            CustomValidator validator = new CustomValidator();
            validator.ValidationGroup = ValidationGroup;
            validator.ServerValidate += OnServerValidate;
            validator.EnableClientScript = false;
            Controls.Add(validator);
        }

        /// <summary>
        /// Raises the validation failed event.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private void RaiseValidationFailedEvent(string msg)
        {
            var threadSafeLocalCopy = ValidationFailed;
            if (threadSafeLocalCopy != null)
                threadSafeLocalCopy(msg);
        }

        /// <summary>
        /// Called when [server validate].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            
            for (Control parentControl = Parent; parentControl != null; parentControl = parentControl.Parent)
            {
                bool doNotValidateIfAnyParentInvisible = !parentControl.Visible;
                if (doNotValidateIfAnyParentInvisible)
                    return;
            }

            bool skipValidation = ValidationSkipCondition != null && ValidationSkipCondition();
            if (skipValidation)
                return;

            var eventArgs = new ValidateEventArgs { IsValid = e.IsValid };
            OnValidate(eventArgs);
            e.IsValid = eventArgs.IsValid;
            if (!eventArgs.IsValid)
            {
                RaiseValidationFailedEvent(eventArgs.Message);
            }
        } 

        #endregion
    }

    /// <summary>
    /// Event arguments for validated control events
    /// </summary>
    public class ValidateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; set; }
    }
}