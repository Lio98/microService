using MicroService.ContentService.Context;
using MicroService.ContentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {

        private ContentContext contentContext;

        public CategoryRepository(ContentContext contentContext) 
        {
            this.contentContext = contentContext;
        }

        public IEnumerable<Category> GetCategorys()
        {
            return contentContext.Category.ToList();
        }

        //public IEnumerable<Category> GetCategorys(int contentId)
        //{
        //    return contentContext.Category.Where(i => i.FUserId == contentId);
        //}

        public Category GetCategoryById(int id)
        {
            return contentContext.Category.Find(id);
        }

        public void Create(Category category)
        {
            contentContext.Category.Add(category);
            contentContext.SaveChanges();
        }
        public void Update(Category category)
        {
            contentContext.Category.Update(category);
            contentContext.SaveChanges();
        }

        public void Delete(Category category)
        {
            contentContext.Category.Remove(category);
            contentContext.SaveChanges();
        }

        public bool CategoryExists(int id)
        {
            return contentContext.Category.Any(i => i.FId == id);
        }

    }
}
