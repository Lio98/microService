using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MicroService.Core.HttpPollyClient
{
    /// <summary>
    /// 微服务中HttpClient熔断，降级策略扩展
    /// </summary>
    public static class PollyHttpClientServiceCollectionExtensions
    {
        /// <summary>
        /// Httpclient扩展方法
        /// </summary>
        /// <param name="services">ioc容器</param>
        /// <param name="name">HttpClient 名称(针对不同的服务进行熔断，降级)</param>
        /// <param name="action">熔断降级配置</param>
        /// <param name="TResult">异常降级</param>
        /// <returns></returns>
        public static IServiceCollection AddPollyHttpClient(this IServiceCollection services, string name, Action<PollyHttpClientOptions> action)
        {
            // 1、创建选项配置类
            PollyHttpClientOptions options = new PollyHttpClientOptions();
            action(options);

            // 1、自定义异常处理(用缓存处理)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("系统正繁忙，请稍后重试"),// 内容，自定义内容
                StatusCode = HttpStatusCode.BadGateway // 504
            };

            services.AddHttpClient(name) // 请求连接复用
                .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<ExecutionRejectedException>().FallbackAsync(fallbackResponse))// 捕获所有的Polly异常
                .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime))) // 断路器
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(options.TimeoutTime)) // 超时
                .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().RetryAsync(options.RetryCount))//失败重试
                .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(10, 100));// 资源隔离（保证每一个服务都是固定的线程）

            return services;
        }
    }
}
