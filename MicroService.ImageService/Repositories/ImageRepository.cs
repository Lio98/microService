using MicroService.ImageService.Context;
using MicroService.ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Repositories
{
    public class ImageRepository: IImageRepository
    {
        private readonly ImageContext imageContext;
        public ImageRepository(ImageContext imageContext) 
        {
            this.imageContext = imageContext;
        }
        public void Create(Image image) 
        {
            imageContext.Images.Add(image);
            imageContext.SaveChanges();
        }
    }
}
