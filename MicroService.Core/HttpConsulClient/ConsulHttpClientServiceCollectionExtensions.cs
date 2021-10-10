using MicroService.Core.LoadBalance;
using MicroService.Core.Mock;
using MicroService.Core.Registry.Extentions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.HttpConsulClient
{
    /// <summary>
    /// HttpClientFactory conusl下的扩展
    /// </summary>
    public static class ConsulHttpClientServiceCollectionExtensions
    {
        /// <summary>
        /// 添加consul
        /// </summary>
        /// <typeparam name="ConsulHttpClient"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientConsul<ConsulHttpClient>(this IServiceCollection services) where ConsulHttpClient : class
        {
            // 1、注册consul
            services.AddConsulDiscovery();

            // 2、注册服务负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 3、注册httpclient
            services.AddSingleton<ConsulHttpClient>();

            // 4、添加服务降级
            services.AddSingleton<IMock, ExceptionReturnMock>();

            return services;
        }
    }
}
