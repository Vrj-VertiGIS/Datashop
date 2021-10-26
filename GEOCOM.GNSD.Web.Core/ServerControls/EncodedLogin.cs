using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GEOCOM.GNSD.Web.Core.ServerControls
{
    /// <summary>
    /// Control derived from the standard ASP.net Login control and encodes the password
    /// </summary>
    public class EncodedLogin : Login
    {
        #region Public Properties
        
        /// <summary>
        /// Gets the TXT password.
        /// </summary>
        protected TextBox TxtPassword
        {
            get
            {
               
                Control passwordControl = FindControl("Password") ;
                if(passwordControl is TextBox)
                {
                    return passwordControl as TextBox;
                }

                if (passwordControl is ILabelAndTextBox  )
                {
                    return (passwordControl as ILabelAndTextBox).TextBox;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the password entered by the user.
        /// </summary>
        /// <returns>The password entered by the user. The default is null.</returns>
        public override string Password
        {
            get
            {
                string decodedPassword = DecodeTxtPassword();
                return decodedPassword;
            }
        }

        /// <summary>
        /// Gets the TXT password client id.
        /// </summary>
        public string TxtPasswordClientId
        {
            get { return TxtPassword.ClientID; }
        } 

        #endregion

        #region Page lifecycle events

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            RegisterEncodeFileJavascirptFileUrl();
            RegisterOnSubmitStatement();
        } 

        #endregion

        #region Protected methods
        
        /// <summary>
        /// Decodes the TXT password.
        /// </summary>
        /// <returns></returns>
        protected string DecodeTxtPassword()
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(TxtPassword.Text);
                string decoded = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                return decoded;
            }
            catch (FormatException)
            {
                return TxtPassword.Text;
            }
        } 

        #endregion

        #region Private methods
        
        /// <summary>
        /// Registers the on submit statement.
        /// </summary>
        private void RegisterOnSubmitStatement()
        {
            string registerKey = Guid.NewGuid().ToString();
            string registerScript = string.Format("encode('{0}')", TxtPasswordClientId);
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), registerKey, registerScript);
        }

        /// <summary>
        /// Registers the encode file javascirpt file URL.
        /// </summary>
        private void RegisterEncodeFileJavascirptFileUrl()
        {
            string encodeFileUrl = Page.ResolveUrl("~/js/encode.js");
            bool registerEncodeFileUrl = (!Page.ClientScript.IsClientScriptIncludeRegistered(encodeFileUrl));
            if (registerEncodeFileUrl)
            {
                Page.ClientScript.RegisterClientScriptInclude(encodeFileUrl, encodeFileUrl);
            }
        } 

        #endregion
    }
}