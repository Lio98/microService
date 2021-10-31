using MicroService.UserService.Models;
using MicroService.UserService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        public UserController(IUserService userService, IConfiguration configuration,ILogger<UserController> logger) 
        {
            this.userService = userService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// 查询所有成员信息
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        public ActionResult<IEnumerable<User>> GetUsers() 
        {
            logger.LogInformation("用户查询开始");
            bool.TryParse(configuration.GetSection("QueryCache").Value,out bool queryCache);
            if (queryCache)
            {
                //查询缓存
                return new List<User>();
            }
            else 
            {
                return userService.GetUsers().ToList();
            }
            

        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public ActionResult<User> GetUser(int id)
        {
            var user = userService.GetUserById(id);
            if (null == user)
            {
                return NotFound();
            }
            return user;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult PutUser(User user) 
        {
            try
            {
                userService.Update(user);
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!userService.UserExists(user.FId))
                {
                    return NotFound();
                }
                else 
                {
                    throw;
                }
            }
            return Ok();
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //[HttpPost, Compensable(nameof(FailDeleteUser))]
        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            userService.Create(user);

            return CreatedAtAction("GetUser", new { id = user.FId }, user);
        }

        void FailDeleteUser(User user) 
        {
            userService.Delete(user);
        }

        [HttpDelete]
        public ActionResult<User> DeleteUser(int id) 
        {
            var user = userService.GetUserById(id);
            if (null == user) 
            {
                return NotFound();
            }
            userService.Delete(user);
            return user;
        }
    }
}
