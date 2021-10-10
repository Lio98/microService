using MicroService.Core.LoadBalance;
using MicroService.Core.Registry.Extentions;
using MicroService.ImageService.Context;
using MicroService.ImageService.Repositories;
using MicroService.ImageService.Services;
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

namespace MicroService.ImageService
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
            // 1��ע�������ĵ�IOC����
            services.AddDbContext<ImageContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2��ע��ͼƬservice
            services.AddScoped<IImageService, ImageServiceImpl>();

            // 3��ע��ͼƬ�ִ�
            services.AddScoped<IImageRepository, ImageRepository>();

            // 4����ӷ���ע������
            services.AddConsulRegistry(Configuration);

            // 5����Ӹ��ؾ���
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            //6�����rabbitmq
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

                // 6.3 ��Ϣ�־û�
                x.UseEntityFramework<ImageContext>();
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));

                x.FailedRetryInterval = 1;//ʧ������ִ�м��ʱ��
                x.FailedRetryCount = 5;// �ٹ��˹�����ֻ�ܽ����˹����� 2���Ǳ��

                // 6.4 �˹������Ǳ��� // �ڲ�aspnetcore
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

            // 1��consul����ע��
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
