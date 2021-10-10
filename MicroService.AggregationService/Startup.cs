using MicroService.AggregationService.Context;
using MicroService.AggregationService.Services;
using MicroService.Core.DistributedTransaction;
using MicroService.Core.HttpConsulClient;
using MicroService.Core.HttpPollyClient;
using MicroService.Core.LoadBalance;
using MicroService.Core.Mock;
using MicroService.Core.Registry.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHttpClient();

            services.AddDbContext<AggregationRabbitmqContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            }/*,ServiceLifetime.Singleton,ServiceLifetime.Singleton*/);

            // 1、创建一个类，加载consul,动态创建
            // 1.1 根据服务名称，创建services.AddHttpClient("UserService");
            services.AddPollyHttpClient("UserService", options => {
                options.TimeoutTime = 60; // 1、超时时间
                options.RetryCount = 3;// 2、重试次数
                options.CircuitBreakerOpenFallCount = 2;// 3、熔断器开启(多少次失败开启)
                options.CircuitBreakerDownTime = 100;// 4、熔断器开启时间
            });
            services.AddPollyHttpClient("ContentService", options => {
                options.TimeoutTime = 60; // 1、超时时间
                options.RetryCount = 3;// 2、重试次数
                options.CircuitBreakerOpenFallCount = 2;// 3、熔断器开启(多少次失败开启)
                options.CircuitBreakerDownTime = 100;// 4、熔断器开启时间
            });

            // 2、注册consul服务发现
            services.AddConsulDiscovery();

            // 3、注册负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 4、注册user服务客户端
            services.AddSingleton<IUserServiceClient, HttpUserServiceClient>();
            services.AddSingleton<IContentServiceClient, HttpContentServiceClient>();

            // 5、注册Consulhttpclient
            services.AddSingleton<ConsulHttpClient>();

            // 6、添加服务注册
            services.AddConsulRegistry(Configuration);

            // 7、添加服务降级
            services.AddSingleton<IMock, ExceptionReturnMock>();

            // 8、注册saga分布式事务集群支持
            //services.AddOmegaCoreCluster("servicecomb-alpha-server", "AggregateService");

            //9、添加rabbitmq
            services.AddCap(x =>
            {
                // 6.1 使用RabbitMQ进行事件中心处理
                x.UseRabbitMQ(rb =>
                {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });
                //使用内存存储消息
                //x.UseInMemoryStorage();

                //消息持久化
                x.UseEntityFramework<AggregationRabbitmqContext>();
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
                x.UseDashboard();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseConsulRegistry();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
