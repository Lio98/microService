using MicroService.ContentService.Models;
using MicroService.ContentService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet("GetCategorys")]
        public ActionResult<IEnumerable<Category>> GetCategorys(int userId)
        {
            return categoryService.GetCategorys().ToList();

        }

        [HttpGet("GetCategory")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        [HttpPut]
        public ActionResult<Category> PutCategory(Category category)
        {
            if (category.FId == 0)
            {
                return BadRequest();
            }
            try
            {
                categoryService.Update(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!categoryService.CategoryExists(category.FId))
                {
                    return NotFound();
                }
                else 
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public ActionResult<Category> PostCategory(Category category) 
        {
            categoryService.Create(category);
            return CreatedAtAction("GetCategory", new { id = category.FId }, category);
        }

        [HttpDelete]
        public ActionResult<Category> DeleteCategory(int id)
        {
            var category = categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            categoryService.Delete(category);
            return category;
        }
    }
}
