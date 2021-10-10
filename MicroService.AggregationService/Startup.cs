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

            // 1������һ���࣬����consul,��̬����
            // 1.1 ���ݷ������ƣ�����services.AddHttpClient("UserService");
            services.AddPollyHttpClient("UserService", options => {
                options.TimeoutTime = 60; // 1����ʱʱ��
                options.RetryCount = 3;// 2�����Դ���
                options.CircuitBreakerOpenFallCount = 2;// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
            });
            services.AddPollyHttpClient("ContentService", options => {
                options.TimeoutTime = 60; // 1����ʱʱ��
                options.RetryCount = 3;// 2�����Դ���
                options.CircuitBreakerOpenFallCount = 2;// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
            });

            // 2��ע��consul������
            services.AddConsulDiscovery();

            // 3��ע�Ḻ�ؾ���
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 4��ע��user����ͻ���
            services.AddSingleton<IUserServiceClient, HttpUserServiceClient>();
            services.AddSingleton<IContentServiceClient, HttpContentServiceClient>();

            // 5��ע��Consulhttpclient
            services.AddSingleton<ConsulHttpClient>();

            // 6����ӷ���ע��
            services.AddConsulRegistry(Configuration);

            // 7����ӷ��񽵼�
            services.AddSingleton<IMock, ExceptionReturnMock>();

            // 8��ע��saga�ֲ�ʽ����Ⱥ֧��
            //services.AddOmegaCoreCluster("servicecomb-alpha-server", "AggregateService");

            //9�����rabbitmq
            services.AddCap(x =>
            {
                // 6.1 ʹ��RabbitMQ�����¼����Ĵ���
                x.UseRabbitMQ(rb =>
                {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });
                //ʹ���ڴ�洢��Ϣ
                //x.UseInMemoryStorage();

                //��Ϣ�־û�
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
