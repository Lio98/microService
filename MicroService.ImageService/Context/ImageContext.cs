using MicroService.ImageService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Context
{
    public class ImageContext:DbContext
    {
        public ImageContext(DbContextOptions<ImageContext> options) : base(options) 
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Image>().ToTable("t_images_image");
        }

        public DbSet<Image> Images { get; set; }
    }
}
