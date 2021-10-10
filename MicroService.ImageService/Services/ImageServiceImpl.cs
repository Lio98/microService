using MicroService.ImageService.Model;
using MicroService.ImageService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Services
{
    public class ImageServiceImpl: IImageService
    {
        private readonly IImageRepository ImageRepository;
        public ImageServiceImpl(IImageRepository imageRepository) 
        {
            this.ImageRepository = imageRepository;
        }
        public void Create(Image image) 
        {
            ImageRepository.Create(image);
        }
    }
}
