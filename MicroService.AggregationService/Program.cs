using MicroService.Core.ConfigCenter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace MicroService.AggregationService
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
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddEnvironmentVariables();// 可以动态替换json配置内容
                        // 1、加载环境变量配置
                        IWebHostEnvironment webHostEnvironment = context.HostingEnvironment;
                        string EnvironmentName = webHostEnvironment.EnvironmentName;// 这个是环境名
                        string ApplicationName = webHostEnvironment.ApplicationName;// 应用名称
                        // 2、根据环境变量加载配置文件
                        IConfiguration configuration = config
                        .AddJsonFile($"appsettings.{EnvironmentName}.json", false, true)
                        .Build(); // 动态监听配置文件改变

                        // 3、从consul配置中心加载配置文件
                        string CongfigCenter = configuration["CongfigCenter"];
                        config.AddConsul($"{ApplicationName}/appsettings.{EnvironmentName}.json", options =>
                        {
                            options.ConsulConfigurationOptions = cco =>
                            {
                                cco.Address = new Uri(CongfigCenter);
                            };

                            // 3.1、热加载配置文件
                            options.ReloadOnChange = true;
                            // 3.2、忽略加载异常(没有配置文件的时候会出现异常)
                            options.OnLoadException = context => context.Ignore = true;
                        });

                        // 4、根据变量加载NLog配置文件
                        NLogBuilder.ConfigureNLog($"nlog.{EnvironmentName}.config");
                    });
                }).UseNLog();
    }
}
