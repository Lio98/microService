using MicroService.ContentService.Models;
using MicroService.ContentService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Services
{
    public class CategoryServiceImpl : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryServiceImpl(ICategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;
        }

        public IEnumerable<Category> GetCategorys()
        {
            return categoryRepository.GetCategorys();
        }

        //public IEnumerable<Category> GetCategorys(int userId)
        //{
        //    return categoryRepository.GetCategorys(userId);
        //}
        public Category GetCategoryById(int id)
        {
            return categoryRepository.GetCategoryById(id);
        }
        
        public void Create(Category category)
        {
            categoryRepository.Create(category);
        }

        public void Update(Category category)
        {
            categoryRepository.Update(category);
        }

        public void Delete(Category category)
        {
            categoryRepository.Delete(category);
        }

        public bool CategoryExists(int id)
        {
            return categoryRepository.CategoryExists(id);
        }

    }
}
