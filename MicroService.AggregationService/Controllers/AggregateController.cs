using DotNetCore.CAP;
using MicroService.AggregationService.Models;
using MicroService.AggregationService.Models.Enum;
using MicroService.AggregationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly IContentServiceClient contentServiceClient;
        private readonly IUserServiceClient userServiceClient;
        private readonly ICapPublisher capPublisher;

        private ILogger logger;

        public AggregateController(IContentServiceClient contentServiceClient,IUserServiceClient userServiceClient, ICapPublisher capPublisher,ILogger<AggregateController> logger)
        {
            this.contentServiceClient = contentServiceClient;
            this.userServiceClient = userServiceClient;
            this.capPublisher = capPublisher;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            logger.LogInformation("聚合服务开启");
            var users = await userServiceClient.GetUsers();
            foreach (var user in users)
            {
                var contents = await contentServiceClient.GetContents(0, ContentSelectType.UserId, user.FId);
                user.FContents = contents;
            }
            logger.LogError("聚合服务结束");
            return Ok(users);
        }

        //[HttpPost, SagaStart]
        [HttpPost]
        public ActionResult Post()
        {
            //添加用户
            var user = new User() { FName = "test", FAge = 23 };
            user = userServiceClient.InsertUsers(user);
            //添加内容
            var content = new Content() { FTitle = "test", FContent = "Hello World!!!", FUserId = user.FId };
            content = contentServiceClient.InsertContents(content);
            Image image = new Image
            {
                FImageUrl = "https://www.baidu.com.test",
                FContentId = content.FId
            };
            capPublisher.PublishAsync<Image>("Image.url", image);
            return Ok("添加成功!");
        }
    }
}
