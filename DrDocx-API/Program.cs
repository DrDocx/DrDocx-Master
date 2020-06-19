using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using DrDocx.API.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace DrDocx.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Paths.EnsureDirsCreated();
            NLogHelper.ConfigureNLog();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddNLog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.Listen(IPAddress.Loopback, 1211);
                        });
                })
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    if (env.IsEnvironment("Local"))
                    {
                        builder.AddUserSecrets<Startup>();
                    }
                });
    }
}