using MicroService.Core.LoadBalance;
using MicroService.Core.Mock;
using MicroService.Core.Registry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.HttpConsulClient
{
    public class ConsulHttpClient
    {
        private readonly IServiceDiscovery serviceDiscovery;
        private readonly ILoadBalance loadBalance;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMock mock;

        public ConsulHttpClient(IServiceDiscovery serviceDiscovery,
                                    ILoadBalance loadBalance,
                                    IHttpClientFactory httpClientFactory,
                                    IMock mock)
        {
            this.serviceDiscovery = serviceDiscovery;
            this.loadBalance = loadBalance;
            this.httpClientFactory = httpClientFactory;
            this.mock = mock;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务协议:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string Serviceshcme, string ServiceName, string serviceLink)
        {
            // 1、降级数据
            mock.DoMock(ServiceName);

            // 故障转移
            string json = "";
            int RestyConut = 0;
            for (int i = 0; i <= 3; i++)
            {
                // 1、是否达到阀值
                if (RestyConut == 3)
                {
                    throw new Exception($"微服务重试操作阀值");
                }
                // 1、获取服务
                IList<ServiceUrl> serviceUrls = await serviceDiscovery.Discovery(ServiceName);

                // 2、负载均衡服务
                ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

                try
                {
                    // 3、建立请求
                    HttpClient httpClient = httpClientFactory.CreateClient(ServiceName);
                    HttpResponseMessage response = await httpClient.GetAsync(Serviceshcme + "://" + serviceUrl.Url + serviceLink);

                    // 3.1、json转换成对象
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    else
                    {
                        throw new Exception($"{ServiceName}服务调用错误，异常{await response.Content.ReadAsStringAsync()}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"异常{e.Message}");
                    // 存储到集合
                    ++RestyConut;
                    Console.WriteLine($"调用微服务{ServiceName}出现故障，开始故障转移{RestyConut}");
                }
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务名称:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <param name="paramData">服务参数</param>
        /// <returns></returns>
        public T PostAsync<T>(string Serviceshcme, string ServiceName, string serviceLink, object paramData = null)
        {
            // 1、获取服务
            IList<ServiceUrl> serviceUrls = serviceDiscovery.Discovery(ServiceName).Result;

            // 2、负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            // 3、建立请求
            Console.WriteLine($"请求路径：{Serviceshcme} +'://'+{serviceUrl.Url} + {serviceLink}");
            HttpClient httpClient = httpClientFactory.CreateClient(ServiceName);

            // 3.1 转换成json内容
            HttpContent hc = new StringContent(JsonConvert.SerializeObject(paramData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.PostAsync(Serviceshcme + "://" + serviceUrl.Url + serviceLink, hc).Result;

            // 3.1json转换成对象
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.2、进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{ServiceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
