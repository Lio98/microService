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
using Servicecomb.Saga.Omega.AspNetCore.Extensions;
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
            //services.AddPollyHttpClient("UserService", options => {
            //    options.TimeoutTime = 60; // 1����ʱʱ��
            //    options.RetryCount = 3;// 2�����Դ���
            //    options.CircuitBreakerOpenFallCount = 2;// 3���۶�������(���ٴ�ʧ�ܿ���)
            //    options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
            //});
            //services.AddPollyHttpClient("ContentService", options => {
            //    options.TimeoutTime = 60; // 1����ʱʱ��
            //    options.RetryCount = 3;// 2�����Դ���
            //    options.CircuitBreakerOpenFallCount = 2;// 3���۶�������(���ٴ�ʧ�ܿ���)
            //    options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
            //});

            services.AddPollyHttpClient("UserService", options => {
                options.TimeoutTime = Configuration.GetSection("HttpClientPolly").GetValue<int>("TimeoutTime"); // 1����ʱʱ��
                options.RetryCount = Configuration.GetSection("HttpClientPolly").GetValue<int>("RetryCount");// 2�����Դ���
                options.CircuitBreakerOpenFallCount = Configuration.GetSection("HttpClientPolly").GetValue<int>("CircuitBreakerOpenFallCount");// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = Configuration.GetSection("HttpClientPolly").GetValue<int>("CircuitBreakerDownTime");// 4���۶�������ʱ��
            });
            services.AddPollyHttpClient("ContentService", options => {
                options.TimeoutTime = Configuration.GetSection("HttpClientPolly").GetValue<int>("TimeoutTime"); // 1����ʱʱ��
                options.RetryCount = Configuration.GetSection("HttpClientPolly").GetValue<int>("RetryCount");// 2�����Դ���
                options.CircuitBreakerOpenFallCount = Configuration.GetSection("HttpClientPolly").GetValue<int>("CircuitBreakerOpenFallCount");// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = Configuration.GetSection("HttpClientPolly").GetValue<int>("CircuitBreakerDownTime");// 4���۶�������ʱ��
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
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = Configuration.GetSection("OmegaCore").GetValue<string>("GrpcServerAddress"); // 1��Э�����ĵ�ַ
                option.InstanceId = Guid.NewGuid().ToString();// 2������ʵ��Id
                option.ServiceName = Configuration.GetSection("OmegaCore").GetValue<string>("ServiceName"); ;// 3����������
            });

            //services.AddOmegaCoreCluster("servicecomb-alpha-server", "AggregateService");

            //9�����rabbitmq
            services.AddCap(x => {
                // 6.1 ʹ��RabbitMQ�����¼����Ĵ���
                x.UseRabbitMQ(rb => {
                    rb.HostName = Configuration.GetSection("Cap").GetValue<string>("RabbitMQ.HostName");
                    rb.UserName = Configuration.GetSection("Cap").GetValue<string>("RabbitMQ.UserName");
                    rb.Password = Configuration.GetSection("Cap").GetValue<string>("RabbitMQ.Password");
                    rb.Port = Configuration.GetSection("Cap").GetValue<int>("RabbitMQ.Port");
                    rb.VirtualHost = Configuration.GetSection("Cap").GetValue<string>("RabbitMQ.VirtualHost");
                });

                // 6.2 ʹ���ڴ�洢��Ϣ(��Ϣ����ʧ�ܴ���)
                x.UseInMemoryStorage();

                // 6.3 ��Ϣ�־û�
                /* x.UseEntityFramework<VideoContext>();
                 x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));*/

                x.FailedRetryInterval = Configuration.GetSection("Cap").GetValue<int>("FailedRetryInterval");
                x.FailedRetryCount = Configuration.GetSection("Cap").GetValue<int>("FailedRetryCount");
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
