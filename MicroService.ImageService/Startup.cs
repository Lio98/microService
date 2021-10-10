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
            // 1、注册上下文到IOC容器
            services.AddDbContext<ImageContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2、注册图片service
            services.AddScoped<IImageService, ImageServiceImpl>();

            // 3、注册图片仓储
            services.AddScoped<IImageRepository, ImageRepository>();

            // 4、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            // 5、添加负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            //6、添加rabbitmq
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

                // 6.3 消息持久化
                x.UseEntityFramework<ImageContext>();
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));

                x.FailedRetryInterval = 1;//失败重新执行间隔时间
                x.FailedRetryCount = 5;// 操过人工次数只能进行人工处理。 2、仪表表

                // 6.4 人工处理仪表盘 // 内部aspnetcore
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

            // 1、consul服务注册
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
