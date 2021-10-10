using MicroService.Core.ConfigCenter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace MicroService.UserService
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
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.ConfigureAppConfiguration((context, config) =>
                    //{
                    //    IConfigCenter configCenter = new ConsulConfigCenter();
                    //    configCenter.LoadConfigCenter(context, config);
                    //});
                }).UseNLog();
    }
}
