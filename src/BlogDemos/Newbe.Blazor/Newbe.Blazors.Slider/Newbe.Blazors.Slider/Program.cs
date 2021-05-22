using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Newbe.Blazors.Slider
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var services = builder.Services;
            services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            services.AddAntDesign();

            services.AddBrowserExtensionServices(options =>
            {
                options.ProjectNamespace = typeof(Program).Namespace;
            });
            await builder.Build().RunAsync();
        }
    }
}