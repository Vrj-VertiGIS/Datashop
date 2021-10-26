using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GEOCOM.GNSD.Common.Config;
using GEOCOM.GNSD.Web.Config;

namespace GEOCOM.GNSD.Web
{
	public class MapProxy : Core.MapProxy.MapProxy
	{
		protected override bool UseMapServiceProxy
		{
			get
			{
				return
				   DatashopWebConfig.Instance.Proxy != null && DatashopWebConfig.Instance.Proxy.UseProxy && DatashopWebConfig.Instance.Proxy.ProxyServices.Any();
			}

		}

		protected override ProxyConfig.ProxyServiceConfig[] ProxyServices
		{
			get { return DatashopWebConfig.Instance.Proxy.ProxyServices; }
		}
	}
}