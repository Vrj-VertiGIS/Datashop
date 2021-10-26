using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace GEOCOM.GNSD.Web.Core.Localization
{
	/// <summary>
	/// Http Module to handle changes in language preference settings
	/// </summary>
	public class LocalizationModule : IHttpModule
	{
		/// <summary>
		/// Handles the PreRequestHandlerExecute event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void context_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["lang"]))
				{
					// Save the selected language as UICulture to the session so that it can be used across all pages
					HttpContext.Current.Session["UICulture"] = HttpContext.Current.Request.QueryString["lang"];
				}

				if (HttpContext.Current.Session != null && HttpContext.Current.Session["UICulture"] != null)
				{
					// Set the current thread's UICulture according to the value from the session
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(HttpContext.Current.Session["UICulture"].ToString());
				}
			}
			catch (Exception)
			{
				//LogManager.GetCurrentClassLogger().Error(ex);
			}
		}

		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application </param>
		public void Init(HttpApplication context)
		{
			context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
		/// </summary>
		public void Dispose()
		{
		}
	}
}