using System;
using System.Collections;
using System.Web;

namespace GEOCOM.GNSD.Web.Common
{
	public class RefreshAction
	{
		public const string PAGEREFRESHENTRY = "PageRefreshEntry";
		public const string CURRENTREFRESHTICKETENTRY = "CurrentRefreshTicketEntry";
		public const string NEXTPAGETICKETENTRY = "NextPageTicketEntry";
		private static Hashtable requestHistory;
		// Hier können weitere Zeichenfolgenkonstanten definiert werden

		public static void Check(HttpContext ctx)
		{
			// Den Ticketplatz initialisieren
			EnsureRefreshTicket(ctx);
			// Das Ticket lesen (aus Session), das in der Sitzung zuletzt gespeichert wurde
			var lastTicket = GetLastRefreshTicket(ctx);
			// Das Ticket der aktuellen Anforderung lesen (aus einem versteckten Feld) 
			var thisTicket = GetCurrentRefreshTicket(ctx, lastTicket);
			// Tickets vergleichen 
			if (thisTicket > lastTicket || (thisTicket == lastTicket && thisTicket == 0))
			{
				UpdateLastRefreshTicket(ctx, thisTicket);
				ctx.Items[PAGEREFRESHENTRY] = false;
			}
			else
			{
				ctx.Items[PAGEREFRESHENTRY] = true;
			}
		}

		// Internen Datenspeicher initialisieren 
		private static void EnsureRefreshTicket(HttpContext ctx)
		{
			if (requestHistory == null)
				requestHistory = new Hashtable();
		}

		// Das für den URL zuletzt ausgegebene Ticket zurückgeben
		private static int GetLastRefreshTicket(HttpContext ctx)
		{
			// Das letzte Ticket extrahieren und zurückgeben 
			if (!requestHistory.ContainsKey(ctx.Request.Path))
				return 0;
			return (int) requestHistory[ctx.Request.Path];
		}

		// Das mit der Seite verknüpfte Ticket zurückgeben
		private static int GetCurrentRefreshTicket(HttpContext ctx, int lastTicket)
		{
			int ticket;
			object o = ctx.Request[CURRENTREFRESHTICKETENTRY];
			if (o == null)
				ticket = lastTicket;
			else
				ticket = Convert.ToInt32(o);

			ctx.Items[NEXTPAGETICKETENTRY] = ticket + 1;
			return ticket;
		}

		// Das für den URL zuletzt ausgegebene Ticket speichern
		private static void UpdateLastRefreshTicket(HttpContext ctx, int ticket)
		{
			requestHistory[ctx.Request.Path] = ticket;
		}
	}
}