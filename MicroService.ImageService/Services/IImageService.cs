using MicroService.ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Services
{
    public interface IImageService
    {
        void Create(Image image);
    }
}
