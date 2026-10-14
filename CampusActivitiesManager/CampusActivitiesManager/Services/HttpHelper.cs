using System.Net.Http.Headers;
using CampusActivitiesManager.Data;

namespace CampusActivitiesManager.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        public AuthTokenHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync(Constants.AuthTokenKey);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // Ignore if SecureStorage is unavailable in test context
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

    public static class HttpHelper
    {
        public static HttpMessageHandler GetInsecureHandler()
        {
            var innerHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            return new AuthTokenHandler(innerHandler);
        }

        public static HttpClient CreateApiClient()
        {
            return new HttpClient(GetInsecureHandler())
            {
                Timeout = TimeSpan.FromSeconds(4)
            };
        }
    }
}
