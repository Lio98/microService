using MicroService.AggregationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Services
{
    public interface IUserServiceClient
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        Task<IList<User>> GetUsers();

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User InsertUsers(User user);
    }
}
