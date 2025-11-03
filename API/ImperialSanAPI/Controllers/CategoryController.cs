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

                return NotFound(new UsualProblemDetails
                {
                    Title = "Ошибка получения категории",
                    Status = StatusCodes.Status404NotFound,
                    Errors = new Dictionary<string, string[]>()
                    {
                        { "Category", ["Такой категории не существует"]}
                    },
                });
            }
        }

        // Получение всех категорий с полным путём
        [HttpGet("get_all_categories_with_path")]
        public IActionResult GetAllCategoriesWithPath()
        {
            using var context = new ImperialSanContext();

            // Загружаем ВСЕ категории один раз
            var allCategories = context.Categories.ToList();

            // Строим словарь: ID → категория
            var categoryDict = allCategories.ToDictionary(c => c.CategoryId);

            // Формируем пути
            var result = allCategories.Select(c => new CategoryPathDTO
            {
                CategoryId = c.CategoryId,
                CategoryTitle = c.CategoryTitle,
                FullPath = BuildPath(c, categoryDict)
            }).ToList();

            return Ok(result);
        }

        private string BuildPath(Category category, Dictionary<int, Category> dict)
        {
            var path = new List<string>();
            var current = category;

            while (current != null)
            {
                path.Add(current.CategoryTitle);
                if (current.ParenCategoryId.HasValue && dict.TryGetValue(current.ParenCategoryId.Value, out var parent))
                {
                    current = parent;
                }
                else
                {
                    break;
                }
            }

            path.Reverse();
            return string.Join(" → ", path);
        }
    }
}
