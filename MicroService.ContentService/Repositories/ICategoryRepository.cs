using MicroService.ContentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetCategorys();

        //IEnumerable<Category> GetCategorys(int contentId);

        Category GetCategoryById(int id);

        void Create(Category category);

        void Update(Category category);

        void Delete(Category category);

        bool CategoryExists(int id);
    }
}
