using ImperialSanAPI.DTOs.CategoryDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // Получить все дочерние категории
        [HttpGet]
        public IActionResult GetCategoryChildrens(int? categoryId = null)
        {
            using (var context = new ImperialSanContext())
            {
                List<CategoryDTO> categories = new List<CategoryDTO>();

                if (categoryId == null)
                {
                    categories = context.Categories.Where(c => c.ParenCategoryId == null)
                                                   .Select(c => new CategoryDTO
                                                   {
                                                       CategoryId = c.CategoryId,
                                                       CategoryTitle = c.CategoryTitle,
                                                   })
                                                   .ToList();

                    return Ok(categories);
                }

                Category? category = context.Categories.Include(c => c.InverseParenCategory)
                                                       .FirstOrDefault(c => c.CategoryId == categoryId);

                if (category != null)
                {
                    categories = category.InverseParenCategory.Select(c => new CategoryDTO
                                                                {
                                                                    CategoryId = c.CategoryId,
                                                                    CategoryTitle = c.CategoryTitle,
                                                                })
                                                               .ToList();

                    return Ok(categories);
                }

                UsualProblemDetails categoryError = new()
                {
                    Title = "Ошибка получения категории",
                    Status = StatusCodes.Status404NotFound,
                    Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                };

                return NotFound(categoryError);
            }
        }
    }
}
