using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Newbe.Blazors.TryNet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(
                sp =>
                {
                    return new HttpClient();
                    var url = builder.HostEnvironment.BaseAddress;
                    var logger = sp.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("baseUri: {Uri}", url);
                    return new HttpClient {BaseAddress = new Uri(url)};
                });

            builder.Services.AddBrowserExtensionServices(options =>
            {
                options.ProjectNamespace = typeof(Program).Namespace;
            });
            await builder.Build().RunAsync();
        }
    }
}