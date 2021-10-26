using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Web.Core.Localization.Language;
using GEOCOM.GNSD.Web.Core.Service;
using System;
using System.Linq;

namespace GEOCOM.GNSD.Web
{
    public partial class ResetPasswordPage : Common.Page
    {
        private IMsg log = new Msg(typeof(ResetPasswordPage));

        protected void BtnResetPasswordOnClick(object sender, EventArgs e)
        {
            if (!captcha.IsValid)
            {
                this.ShowMessage(this.captcha.ErrorMessage);
                return;
            }

            if (!Page.IsValid)
                return;

            try
            {
                var user = DatashopService.Instance.JobService.GetUserByEmail(email.Text).FirstOrDefault(u => u.BizUser != null);
                if (user?.BizUser != null)
                {
                    DatashopService.Instance.JobService.SendResetPasswordMail(user);
                }

                // show the message even though the user does not exist - preventing user guessing
                var infoText = WebLanguage.LoadStr(2404, "Password reset email sent to mail {0}.");
                string message = String.Format(infoText, email.Text);
                this.ShowMessage(message);

                email.Text = null;
                captcha.Refresh();
            }
            catch (Exception exp)
            {
                log.Error("Could not reset Password for User " + email.Text, exp);
                this.ShowMessage(WebLanguage.LoadStr(2405, "Password could not be reset. Please try later or contact an admin."));
            }
        }

    }
}