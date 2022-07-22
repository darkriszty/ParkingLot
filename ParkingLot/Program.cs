using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ParkingLot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var startup = new Startup();

            Startup.ConfigureServices(builder.Services, builder.Configuration);

            var webApplication = builder.Build();
            startup.Configure(webApplication, builder.Environment);

            var logger = webApplication.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                await webApplication.RunAsync();
            }
            catch (Exception err)
            {
                logger.LogError(err, "Unhandled exception");
            }
        }
    }
}