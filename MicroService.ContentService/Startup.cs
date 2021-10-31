using MicroService.ContentService.Context;
using MicroService.ContentService.Repositories;
using MicroService.ContentService.Services;
using MicroService.Core.DistributedTransaction;
using MicroService.Core.LoadBalance;
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

namespace MicroService.ContentService
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
            services.AddDbContext<ContentContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            }/*, ServiceLifetime.Singleton, ServiceLifetime.Singleton*/);
            // 2��ע������service
            services.AddScoped<IContentService, ContentServiceImpl>();
            services.AddScoped<ICategoryService, CategoryServiceImpl>();

            // 3��ע�����ݲִ�
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // 4����ӷ���ע������
            services.AddConsulRegistry(Configuration);

            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 5��ע��saga�ֲ�ʽ����Ⱥ֧��
            services.AddOmegaCore(option =>
            {
                //option.GrpcServerAddress = "localhost:8081"; // 1��Э�����ĵ�ַ
                //option.InstanceId = Guid.NewGuid().ToString();// 2������ʵ��Id
                //option.ServiceName = "ContentService";// 3����������
                option.GrpcServerAddress = Configuration.GetSection("OmegaCore").GetValue<string>("GrpcServerAddress"); // 1��Э�����ĵ�ַ
                option.InstanceId = Guid.NewGuid().ToString();// 2������ʵ��Id
                option.ServiceName = Configuration.GetSection("OmegaCore").GetValue<string>("ServiceName");// 3����������
            });
            //services.AddOmegaCoreCluster("servicecomb-alpha-server", "ContentService");

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
