using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Ocelot.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;

namespace MicroService.Gateway
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
                    webBuilder.ConfigureAppConfiguration((context, config) =>
                    {
                    //config.AddJsonFile("ocelot.json", true, true);
                    ////config.AddOcelot(hostingContext.HostingEnvironment);
                    ///webBuilder.ConfigureAppConfiguration((context, config) =>
                        // ʹ�û������������ã�(�滻json�ļ����������)
                        config.AddEnvironmentVariables();
                        // 1�����ػ�����������
                        IWebHostEnvironment webHostEnvironment = context.HostingEnvironment;
                        string EnvironmentName = webHostEnvironment.EnvironmentName;// ����ǻ�����
                        string ApplicationName = webHostEnvironment.ApplicationName;// Ӧ������
                        // 2�����ݻ����������������ļ�
                        IConfiguration configuration = config
                        .AddJsonFile($"appsettings.{EnvironmentName}.json", false, true)
                        .AddJsonFile($"ocelot-dynamic.{EnvironmentName}.json", false, true)
                        .Build(); // ��̬���������ļ��ı�

                        // 3����consul�������ļ��������ļ�
                        string CongfigCenter = configuration["CongfigCenter"];
                        config.AddConsul($"{ApplicationName}/appsettings.{EnvironmentName}.json", options =>
                        {
                            options.ConsulConfigurationOptions = cco =>
                            {
                                cco.Address = new Uri(CongfigCenter);
                            };

                            // 3.1���ȼ��������ļ�
                            options.ReloadOnChange = true;
                            // 3.2�����Լ����쳣(û�������ļ���ʱ�������쳣)
                            options.OnLoadException = context => context.Ignore = true;
                        });

                        // 4�����ݱ�������NLog�����ļ�
                        NLogBuilder.ConfigureNLog($"nlog.{EnvironmentName}.config");
                    
                });
                }).UseNLog();
    }
}
