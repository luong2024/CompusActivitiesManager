using System.Net.Http;

namespace CampusActivitiesManager.Services
{
    public static class HttpHelper
    {
        public static HttpMessageHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
        }
    }
}
