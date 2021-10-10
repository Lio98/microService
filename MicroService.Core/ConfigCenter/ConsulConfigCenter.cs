using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Winton.Extensions.Configuration.Consul;

namespace MicroService.Core.ConfigCenter
{
    /// <summary>
    /// consul配置中心
    /// </summary>
    public class ConsulConfigCenter : IConfigCenter
    {
        public void LoadConfigCenter(WebHostBuilderContext context, IConfigurationBuilder config)
        {
            // 1、加载配置中心配置文件
            IConfiguration configuration = config.AddJsonFile("configcenter.json", false, true).Build(); // 动态监听配置文件改变

            // 2、取值
            string CongfigCenter = configuration["CongfigCenter"];

            // 3、加载consul配置文件
            IWebHostEnvironment webHostEnvironment = context.HostingEnvironment;
            string EnvironmentName = webHostEnvironment.EnvironmentName;// 这个是环境名
            string ApplicationName = webHostEnvironment.ApplicationName;// 应用名称
            config.AddConsul($"{ApplicationName}/appsettings.{EnvironmentName}.json", options =>
            {
                options.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(CongfigCenter);
                };

                // 1、热加载配置文件
                options.ReloadOnChange = true;
            });
        }

    }
}
