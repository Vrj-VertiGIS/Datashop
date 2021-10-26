using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Common.Logging;

namespace GEOCOM.GNSD.Common.Mail
{
    public class MailSender
    {
        private static IMsg _log = null;
        private Dictionary<string, string> _variables = null;

        public MailSender()
        {
            InitLogger();
        }

        /// <summary>
        /// This Method sends a Mail after repleacing the placeholders in the Mailtemplate with the variables.
        /// The caller has to make sure, that he adds all necessary Key/Values to the Dictionary that he will pass.
        /// The only entry in the Dictonary that will be added in this Method is for %(nl) = NewLine.
        /// If an entry is missing, the mail will be sent anyway, but the placeholders will still be in the text.
        /// The mail will be send either asynchronous or synchronous depending on the parameter sendAsync.
        /// Asynchronous: The process it returns in an instant, but you can't be sure if mail was realy sent.
        ///               Check the logs for seeing if mail was sent.
        /// Synchronous:  The mail will be sent synchronous, so you have to wait until it's finished,
        ///               but you will know if its was sent sucsessfully.
        ///               A timeout (maximum wait time) can be specified for each server in the configuration file.
        /// </summary>
        /// <param name="mailTemplateName">Name of th Template specified in GlobalConfigFile</param>
        /// <param name="toAdress">Adress of the reciver</param>
        /// <param name="variables">A Dictionary containing all Key/Value-Pairs for replacing the placeholders in the mailtemplate.</param>
        /// <param name="sendAsync">Should the mail be sent asynchronus or synchronous?</param>
        public void SendMail(string mailTemplateName, string toAdress, Dictionary<string, string> variables, bool sendAsync)
        {
            string fromMailAddressString = string.Empty;
            string toMailAddressString = string.Empty;

            try
            {
                if (!GnsDatashopCommonConfig.Instance.Mail.IsEnabled)
                {
                    _log.DebugFormat("Mailing is disabled. To enable mailing, set 'enabled' attribute in mail configuration to 'true'.");
                    return;
                }

                _log.DebugFormat("Trying to construct a Mail with template {0} to address {1}", mailTemplateName, toAdress);
                if (variables != null)
                {
                    _variables = variables;
                }

                try
                {
                    var a = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate[mailTemplateName];
                }
                catch (KeyNotFoundException)
                {
                    _log.ErrorFormat("Mailtemplate {0} was not found. Sending mail aborted", mailTemplateName);
                    throw;
                }

                fromMailAddressString = GetFromOnlyMailAddressString(mailTemplateName);
                toMailAddressString = toAdress;

                MailAddress fromMailAddress = GetFromMailAddress(mailTemplateName);
                MailAddress toMailAddress = new MailAddress(toAdress);

                // Specify the message content.
                MailMessage message = new MailMessage(fromMailAddress, toMailAddress);
                message.BodyEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                message.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");

                // Replace Vars and set Subject
                message.Subject = ReplaceVariables(GnsDatashopCommonConfig.Instance.Mail.Mailtemplate[mailTemplateName].Subject);

                // Add Variable for newLine
                _variables["nl"] = Environment.NewLine;

                // GetText
                StringBuilder plainText = new StringBuilder();
                foreach (string line in GnsDatashopCommonConfig.Instance.Mail.Mailtemplate[mailTemplateName].Body)
                {
                    plainText.Append(line);
                    plainText.Append("%(nl)");
                }

                // Replace all Vars and set messagebody
                message.Body = ReplaceVariables(plainText.ToString());

                // Create HTML view
                _variables["nl"] = "<br/>";

                StringBuilder htmlBody = new StringBuilder();
                htmlBody.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                htmlBody.Append("<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=UTF-8\">");
                htmlBody.Append("</HEAD><BODY><DIV>");

                htmlBody.Append(ReplaceVariables(plainText.ToString()));

                htmlBody.Append("</DIV></BODY></HTML>");

                // Add the alternate html body to the message.
                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(htmlBody.ToString(), System.Text.Encoding.UTF8, mimeType.ToString());
                message.AlternateViews.Add(alternate);

                message.IsBodyHtml = true;

                SendMail(message, 0, 0, sendAsync, new Exception());
                return;
            }
            catch (Exception exp)
            {
                _log.ErrorFormat("Mail could not be sent. From {0}, To {1}, Error: {2}", fromMailAddressString, toMailAddressString, exp);
                throw;
            }
        }

        private string ReplaceVariables(string p)
        {
            return ReplaceVariables(p, _variables);
        }

        private string ReplaceVariables(string p, Dictionary<string, string> variables)
        {
            foreach (KeyValuePair<string, string> var in _variables)
            {
                string varName = "%(" + var.Key + ")";
                if (p.Contains(varName)) p = p.Replace(varName, var.Value);
            }
            if (p.Contains("%("))
            {
                _log.ErrorFormat("Could not replace all variables. Will send Mail anyway.");
            }
            return p;
        }

        private void SendMail(MailMessage msg, int serverOrder, int attempt, bool sendAsync, Exception error)
        {
            // check if a server with given Order exists, or select the one with the next higher Order
            MailServerInfo.MailServer server = null;
            int high = int.MaxValue;
            foreach (var tempServer in GnsDatashopCommonConfig.Instance.Mail.Servers)
            {
                if (tempServer.Order == serverOrder)
                {
                    server = tempServer;
                    break;
                }
                if (tempServer.Order < high && tempServer.Order > serverOrder)
                {
                    high = tempServer.Order;
                    server = tempServer;
                }
            }
            if (server == null)
            {
                // throw exception to abort step
                throw new Exception("No working Mailserver found. Check configuration!");
            }
            _log.InfoFormat("Select Mailserver {0}:{1}", server.Server, server.Port);

            // Check Attempts
            if (attempt >= server.Retry)
            {
                _log.WarnFormat("Maximum attemts for mailserver {0}:{1} reached. Will switch to next one.", server.Server, server.Port);
                serverOrder = server.Order + 1;
                SendMail(msg, serverOrder, 0, sendAsync, error);
                return;
            }
            attempt++;

            // Get simple mail transfer protocol client ip port
            SmtpClient client = new SmtpClient(server.Server, server.Port);

            // Set username and password for the smtp client, if no user is configured, the mail is sent with the default login credentials
            // but from ArcGisServer, the AgsSOC will usually not have permission to sent mails.
            if (server.SmtpUser == string.Empty)
            {
                client.Credentials = null;
                client.UseDefaultCredentials = false;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(server.SmtpUser, server.SmtpPassword, server.SmtpDomain);
                client.EnableSsl = server.UseSSL;
            }

            client.Timeout = server.Timeout;

            // Now finally send the mail
            bool resendMail = Send(client, msg, serverOrder, attempt, sendAsync);
            if (resendMail)
            {
                SendMail(msg, serverOrder, attempt, sendAsync, new Exception());
            }
        }

        /// <summary>
        /// Sends the mail synchroneous.
        /// SMTP Exceptions are caught and logged. Other exceptions are passed to caller.
        /// </summary>
        /// <param name="client">The SMTP client</param>
        /// <param name="msg">The mail message</param>
        /// <param name="serverOrder">The server order. Multiple servers may be configured.</param>
        /// <param name="attempt">The attempt number</param>
        /// <param name="sendAsync">True if the mail should be sent asynchronous, false otherwise</param>
        /// <returns>True if the mail should be resent, false otherwise</returns>
        private bool Send(SmtpClient client, MailMessage msg, int serverOrder, int attempt, bool sendAsync)
        {
            try
            {
                if (sendAsync)
                {
                    using (new SynchronizationContextSwitcher())
                    {
                        client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                        object[] userState = { msg, serverOrder, attempt, sendAsync };
                        client.SendAsync(msg, userState);
                    }
                }
                else
                {
                    client.Send(msg);
                    _log.InfoFormat("A Mail with subject '{0}' has been sent to {1}.", msg.Subject, msg.To[0].Address);
                }
            }
            catch (SmtpFailedRecipientsException smtpFailedRecipientsException)
            {
                // Only catch the SMTP Exceptions. Other exception will be caught outside.
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format(
                                            "A Mail with subject '{0}' to {1} was sent but at least one recipient is unknown! Failed recipient={2}",
                                            msg.Subject,
                                            msg.To[0].Address,
                                            smtpFailedRecipientsException.FailedRecipient));
                sb.AppendLine("Will NOT try to send the Mail again.");
                sb.AppendLine(string.Format("Exception={0}", smtpFailedRecipientsException.Message));
                _log.Error(sb.ToString());
            }
            catch (SmtpException smtpException)
            {
                _log.ErrorFormat(
                                 "A Mail with subject '{0}' to {1} could NOT been sent. Will try again. {2}",
                                 msg.Subject,
                                 msg.To[0].Address,
                                 smtpException.ToString());
                return true;
            }
            return false;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            object[] vars = e.UserState as object[];
            if (e.Error == null)
            {
                _log.InfoFormat("An asynchronous Mail with subject '{0}' has been sent to {1}.", (vars[0] as MailMessage).Subject, (vars[0] as MailMessage).To[0].Address);
            }
            else
            {
                _log.ErrorFormat("An asynchronous Mail with subject '{0}' to {1} could NOT been sent. Will try again. {2}", (vars[0] as MailMessage).Subject, (vars[0] as MailMessage).To[0].Address, e.Error);
                SendMail((vars[0] as MailMessage), (int)vars[1], (int)vars[2], (bool)vars[3], e.Error);
            }
        }

        private void InitLogger()
        {
            try
            {
                if (_log == null)
                {
                    DatashopLogInitializer.Initialize();

                    // Log4net generic logger interface
                    _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);
                }
            }
            catch (Exception e)
            {
                throw new Exception("LOG-4-NET configuration error", e);
            }
        }

        private static MailAddress GetFromMailAddress(string mailTemplateName)
        {
            //NOTE: Anne Karger wants a displayname setting
            //you can only set it in the ctor so this is where you'd do it :) (MYP)
        
            return new MailAddress(GetFromOnlyMailAddressString(mailTemplateName));
        }

        private static string GetFromOnlyMailAddressString(string mailTemplateName)
        {
            string from = GnsDatashopCommonConfig.Instance.Mail.From;
            string fromOverloaded = GnsDatashopCommonConfig.Instance.Mail.Mailtemplate[mailTemplateName].From;            
            
            if (string.IsNullOrEmpty(fromOverloaded))
            {
                return from;
            }
            
            return fromOverloaded;
        }

        /// <summary>
        /// This is a helpermethode to switch the context. With this the Mail will no longer be sent in the asp Context. If you don't do that, the webservice wont return until the mail is sent completly, which results in a wait of 5-10sec befor the page is rendered again.
        /// Drawback is, that you can't realy be sure if the mail was sent succsesfully. Lock in the logs, errors and succses will be statet there.
        /// InnerClass borowed from Richard at http://haacked.com/archive/2009/01/09/asynchronous-fire-and-forget-with-lambdas.aspx
        /// </summary>
        private class SynchronizationContextSwitcher : IDisposable
        {
            private readonly SynchronizationContext _oldContext;
            private readonly SynchronizationContext _newContext;
            private ExecutionContext _executionContext;

            public SynchronizationContextSwitcher()
                : this(new SynchronizationContext())
            {
            }

            private SynchronizationContextSwitcher(SynchronizationContext context)
            {
                _newContext = context;
                _executionContext = Thread.CurrentThread.ExecutionContext;
                _oldContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(context);
            }

            public void Dispose()
            {
                if (null != _executionContext)
                {
                    if (_executionContext != Thread.CurrentThread.ExecutionContext)
                        throw new InvalidOperationException("Dispose called on wrong thread.");

                    if (_newContext != SynchronizationContext.Current)
                        throw new InvalidOperationException("The SynchronizationContext has changed.");

                    SynchronizationContext.SetSynchronizationContext(_oldContext);
                    _executionContext = null;
                }
            }
        }
    }
}
