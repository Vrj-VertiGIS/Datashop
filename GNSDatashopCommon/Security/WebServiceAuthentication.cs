using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;

using GEOCOM.GNSD.Common.Config;

namespace GEOCOM.GNSD.Common.Security
{
    public class WebServiceAuthentication
    {
        public static void AuthenticateRequest(ServiceEndpoint serviceEndpoint, HttpWebRequest request, Config.Security security)
        {
            BasicHttpBinding basicHttpBinding = (BasicHttpBinding)serviceEndpoint.Binding;
            if (basicHttpBinding.Security.Mode == BasicHttpSecurityMode.TransportCredentialOnly)
            {
                if (security == null || security.Authentication == null)
                {
                    throw new ConfigException("<security> tag in configuration is either missing or invalid");
                }
                NetworkCredential myNetworkCredential = new NetworkCredential(security.Authentication.Username, security.Authentication.Password);

                CredentialCache myCredentialCache = new CredentialCache { { request.RequestUri, "Basic", myNetworkCredential } };

                request.PreAuthenticate = true;
                request.Credentials = myCredentialCache;
            }
            else
            {
                request.Credentials = CredentialCache.DefaultCredentials;                
            }
        }
    }
}
