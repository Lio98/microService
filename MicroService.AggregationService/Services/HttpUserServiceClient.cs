using MicroService.AggregationService.Models;
using MicroService.Core.HttpConsulClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Services
{
    public class HttpUserServiceClient : IUserServiceClient
    {
        private readonly string ServiceSchme = "https";
        private readonly string ServiceName = "UserService"; //服务名称

        // httpclient consul请求
        private readonly ConsulHttpClient consulHttpClient;
        public HttpUserServiceClient(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }

        public async Task<IList<User>> GetUsers()
        {
            string ServiceLink = "/api/User/GetUsers"; //查询所有的用户信息
            List<User> users = await consulHttpClient.GetAsync<List<User>>(ServiceSchme, ServiceName, ServiceLink);
            return users;
        }

        public User InsertUsers(User user)
        {
            string ServiceLink = "/api/User";
            return consulHttpClient.PostAsync<User>(ServiceSchme, ServiceName, ServiceLink, user);
        }
    }
}
