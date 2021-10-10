using DotNetCore.CAP;
using MicroService.ImageService.Model;
using MicroService.ImageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService imageService;
        public ImageController(IImageService imageService) 
        {
            this.imageService = imageService;
        }

        [NonAction]//异步操作，不被webapi管理
        [CapSubscribe("Image.url")]
        public ActionResult<Image> PostImage(Image image) 
        {
            imageService.Create(image);
            Console.WriteLine("添加图片");
            return CreatedAtAction("GetImage", new { id = image.FId }, image);
        }
    }
}
