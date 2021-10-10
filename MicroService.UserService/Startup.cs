using MicroService.UserService.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MicroService.UserService.Services;
using MicroService.UserService.Repositories;
using MicroService.Core.Registry.Extentions;
using MicroService.Core.ConfigCenter;
using MicroService.Core.DistributedTransaction;
using MicroService.Core.LoadBalance;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.UserService
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
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2、注册用户service
            services.AddScoped<IUserService, UserServiceImpl>();

            // 3、注册用户仓储
            services.AddScoped<IUserRepository, UserRepository>();

            // 4、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 5、注册saga分布式事务集群支持
            //services.AddOmegaCore(option =>
            //{
            //    option.GrpcServerAddress = "localhost:8081"; // 1、协调中心地址
            //    option.InstanceId = Guid.NewGuid().ToString();// 2、服务实例Id
            //    option.ServiceName = "UserService";// 3、服务名称
            //});
            //services.AddOmegaCoreCluster("servicecomb-alpha-server", "UserService");

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
