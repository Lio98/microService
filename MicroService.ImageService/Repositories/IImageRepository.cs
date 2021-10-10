using MicroService.ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Repositories
{
    public interface IImageRepository
    {
        void Create(Image image);
    }
}
