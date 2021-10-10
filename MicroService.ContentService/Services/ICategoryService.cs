using MicroService.ContentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Services
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategorys();

        //IEnumerable<Category> GetCategorys(int userId);

        Category GetCategoryById(int id);

        void Create(Category category);

        void Update(Category category);

        void Delete(Category category);

        bool CategoryExists(int id);
    }
}
