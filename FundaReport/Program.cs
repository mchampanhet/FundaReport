using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace FundaReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(Option =>
                    {
                        Option.AddServerHeader = false;
                        Option.Listen(IPAddress.Any, 5001);
                    }).UseStartup<Startup>();
                });
    }
}
