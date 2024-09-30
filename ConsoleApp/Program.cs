using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceLayer;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build the host with dependency injection
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddServiceLayer();
                })
                .Build();

            // Resolve and use the service
            var service = host.Services.GetRequiredService<IService>();
            service.ProcessData();
        }
    }
}