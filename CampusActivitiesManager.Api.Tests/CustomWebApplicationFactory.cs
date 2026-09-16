using CampusActivitiesManager.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CampusActivitiesManager.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public InMemoryAccountService AccountService { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseAccountService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton<IFirebaseAccountService>(AccountService);
            });
        }
    }
}
