using MicroService.ContentService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Context
{
    public class ContentContext: DbContext
    {
        public ContentContext(DbContextOptions<ContentContext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("t_conts_category");
            modelBuilder.Entity<Content>().ToTable("t_conts_content");
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<Content> Content { get; set; }
    }
}
