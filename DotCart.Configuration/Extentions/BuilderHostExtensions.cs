using Microsoft.Extensions.Hosting;
using Serilog;

namespace DotCart.Configuration.Extentions
{
    public class BuilderHostExtensions
    {
        public static void RegisterHost(IHostBuilder host)
        {
            //Serilog is configured before adding dependencies to the DI container
            //This is done to ensure that logging is properly set up as early as possible in the application's lifecycle
            ConfigureSerilog(host);
        }

        private static void ConfigureSerilog(IHostBuilder host)
        {
            host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .WriteTo.Console();
            });
        }
    }
}
