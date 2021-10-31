using MicroService.ContentService.Models;
using MicroService.ContentService.Models.Enum;
using MicroService.ContentService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IContentService contentService;
        private readonly ILogger logger;

        public ContentController(IContentService contentService, ILogger<ContentController> logger) 
        {
            this.contentService = contentService;
            this.logger = logger;
        }

        [HttpGet("GetContents")]
        public ActionResult<IEnumerable<Content>> GetContents(int id, ContentSelectType selectType,int selectTypeId)
        {
            logger.LogInformation("内容查询开始");
            switch (id) 
            {
                case 0:
                    switch (selectTypeId) 
                    {
                        case 0:
                            return contentService.GetContents().ToList();
                        default:
                            return contentService.GetContents(id, selectType, selectTypeId).ToList();
                    }
                default:
                    return contentService.GetContents(id, selectType, selectTypeId).ToList();
                    
            }
        }

        [HttpGet("GetContent")]
        public ActionResult<Content> GetContent(int id)
        {
            logger.LogInformation($"内容查询开始:ID={id}");
            var content = contentService.GetContentById(id);
            if (content == null)
            {
                return NotFound();
            }
            return content;
        }

        [HttpPut]
        public ActionResult<Content> PutContent(Content content)
        {
            if (content.FId == 0)
            {
                return BadRequest();
            }
            try
            {
                contentService.Update(content);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!contentService.ContentExists(content.FId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        Content withDeleteContent;
        //[HttpPost, Compensable(nameof(FailDeleteContent))]
        [HttpPost]
        public ActionResult<Content> PostContent(Content content)
        {
            withDeleteContent = null;
            contentService.Create(content);
            withDeleteContent = content;
            return CreatedAtAction("GetContent", new { id = content.FId }, content);
        }

        [HttpDelete]
        public ActionResult<Content> DeleteContent(int id)
        {
            var content = contentService.GetContentById(id);
            if (content == null)
            {
                return NotFound();
            }
            contentService.Delete(content);
            return content;
        }

        void FailDeleteContent() 
        {
            contentService.Delete(withDeleteContent);
        }
    }
}
