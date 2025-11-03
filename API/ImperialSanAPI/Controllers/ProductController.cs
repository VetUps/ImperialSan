using ImperialSanAPI.DTOs.ProductDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // Получение всех товаров
        [HttpGet]
        public ActionResult<CatalogProductPaginationDTO> GetProducts(
            int pageNumber, 
            int pageSize, 
            int? categoryId = null,
            string? sortBy = null,
            string? sortOrder = "asc")
        {

            var validSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "price", "name", "date", "title"
            };

            var validSortOrders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "asc", "desc"
            };

            if (!string.IsNullOrEmpty(sortBy) && !validSortFields.Contains(sortBy))
            {
                return BadRequest(new UsualProblemDetails
                {
                    Title = "Ошибка сортировки",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = new Dictionary<string, string[]>
                    {
                        { "SortBy",  ["Недопустимое поле для сортировки"] }
                    }
                });
            }

            if (!string.IsNullOrEmpty(sortOrder) && !validSortOrders.Contains(sortOrder))
            {
                return BadRequest(new UsualProblemDetails
                {
                    Title = "Ошибка сортировки",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = new Dictionary<string, string[]>
                    {
                        { "SortOrder", ["Направление сортировки должно быть 'asc' или 'desc'"] }
                    }
                });
            }

            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (pageNumber <=0)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка пагинации",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "PageNumber", ["Такой страницы не существует"]}
                        },
                    });
                }

                if (pageSize <=0)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка пагинации",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "PageSize", ["Размер страницы недопустим"]}
                        },
                    });
                }

                var products = context.Products.Include(p => p.Category).ToList();

                if (categoryId != null)
                {
                    Category categoty = context.Categories.Include(c => c.InverseParenCategory).First(c => c.CategoryId == categoryId);
                    var categoryIds = GetAllCategoryIdsIncludingChildren(categoty.CategoryId, context);

                    products = products.Where(p => categoryIds.Contains((int)p.CategoryId))
                                       .ToList();
                }

                var query = products.AsQueryable();

                if (!string.IsNullOrEmpty(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch
                    {
                        "price" => sortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),

                        "name" or "title" => sortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(p => p.ProductTitle) : query.OrderBy(p => p.ProductTitle),

                        "date" => sortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(p => p.DateOfCreate) : query.OrderBy(p => p.DateOfCreate),
                        
                        _ => query.OrderBy(p => p.ProductId)
                    };
                }
                else
                {
                    query = query.OrderBy(p => p.ProductId);
                }

                int totalCount = products.Count();
                var resultProducts = query.Select(p => new CatalogProductDTO
                                                {
                                                    ProductId = p.ProductId,
                                                    ProductTitle = p.ProductTitle,
                                                    ProductDescription = p.ProductDescription,
                                                    Price = p.Price,
                                                    QuantityInStock = p.QuantityInStock,
                                                    ImageUrl = p.ImageUrl,
                                                    CategoryId = p.CategoryId,
                                                    BrandTitle = p.BrandTitle,
                                                    DateOfCreate = p.DateOfCreate,
                                                    IsActive = p.IsActive,
                                                    CategoryName = p.Category.CategoryTitle,
                                                    ParentCategoryId = p.Category.ParenCategoryId
                                                })
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

                CatalogProductPaginationDTO catalogProductPagination = new CatalogProductPaginationDTO
                {
                    Products = resultProducts,
                    TotalProductsCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(catalogProductPagination);
            }
        }


        // Получение всех брендов товаров
        [HttpGet("get_all_brands")]
        public ActionResult<List<string>> GetProductBrands()
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var productBrands = context.Products.Select(p => p.BrandTitle).Distinct().ToList();

                return Ok(productBrands);
            }
        }

        private List<int> GetAllCategoryIdsIncludingChildren(int rootCategoryId, ImperialSanContext context)
        {
            // Загружаем ВСЕ категории один раз
            var allCategories = context.Categories.ToList();

            // Строим словарь: ParentId -> список детей
            var childrenMap = allCategories
                .Where(c => c.ParenCategoryId.HasValue)
                .GroupBy(c => c.ParenCategoryId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Рекурсивно собираем ID (уже в памяти!)
            var result = new List<int>();
            CollectCategoryIdsRecursive(rootCategoryId, childrenMap, result);
            return result;
        }

        private void CollectCategoryIdsRecursive(int categoryId, Dictionary<int, List<Category>> childrenMap, List<int> result)
        {
            result.Add(categoryId);

            if (childrenMap.TryGetValue(categoryId, out var children))
            {
                foreach (var child in children)
                {
                    CollectCategoryIdsRecursive(child.CategoryId, childrenMap, result);
                }
            }
        }

        // Получение товара
        [HttpGet("{productId}")]
        public ActionResult<CatalogProductDTO> GetProduct(int productId)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products
                                     .Include(p => p.Category)
                                     .FirstOrDefault(p => p.ProductId == productId);

                if (product == null)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    });
                }

                CatalogProductDTO productDTO = new CatalogProductDTO
                {
                    ProductId = product.ProductId,
                    ProductTitle = product.ProductTitle,
                    ProductDescription = product.ProductDescription,
                    Price = product.Price,
                    QuantityInStock = product.QuantityInStock,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    BrandTitle = product.BrandTitle,
                    DateOfCreate = product.DateOfCreate,
                    IsActive = product.IsActive,
                    CategoryName = product.Category.CategoryTitle,
                    ParentCategoryId = product.Category.ParenCategoryId
                };

                return Ok(productDTO);
            }
        }

        // Добавление нового товара
        [HttpPost("add_product")]
        public ActionResult<CatalogProductDTO> Post([FromBody] AddProductDTO addProductDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (context.Products.Where(p => p.BrandTitle == addProductDto.BrandTitle).Any(u => u.ProductTitle == addProductDto.ProductTitle))
                {
                    return Conflict(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Товар с таким названием у этого бренда уже есть"]}
                        },
                    });
                }

                if (!context.Categories.Any(c => c.CategoryId ==  addProductDto.CategoryId))
                {
                    return Conflict(new UsualProblemDetails
                    {
                        Title = "Ошибка получения кагетории",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                    });
                }

                var product = new Product
                {
                    ProductId = context.Products.Count(),
                    ProductTitle = addProductDto.ProductTitle,
                    ProductDescription = addProductDto.ProductDescription,
                    Price = addProductDto.Price,
                    QuantityInStock = addProductDto.QuantityInStock,
                    ImageUrl = addProductDto.ImageUrl,
                    CategoryId = addProductDto.CategoryId,
                    BrandTitle = addProductDto.BrandTitle,
                    DateOfCreate = addProductDto.DateOfCreate,
                    IsActive = addProductDto.IsActive,
                };

                context.Products.Add(product);
                context.SaveChanges();

                return Ok();
            }
        }

        // Редактирование товара
        [HttpPut("update_product")]
        public IActionResult Put([FromBody] UpdateProductDTO updateProductDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.Find(updateProductDto.ProductId);
                if (product == null)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    });
                }

                if (context.Products.Where(p => p.BrandTitle == updateProductDto.BrandTitle && p.ProductId != updateProductDto.ProductId)
                                    .Any(u => u.ProductTitle == updateProductDto.ProductTitle))
                {
                    return Conflict(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Товар с таким названием у этого бренда уже есть"]}
                        },
                    });
                }

                if (!context.Categories.Any(c => c.CategoryId == updateProductDto.CategoryId))
                {
                    return Conflict(new UsualProblemDetails
                    {
                        Title = "Ошибка получения кагетории",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                    });
                }

                product.ProductTitle = updateProductDto.ProductTitle;
                product.ProductDescription = updateProductDto.ProductDescription;
                product.Price = updateProductDto.Price;
                product.QuantityInStock = updateProductDto.QuantityInStock;
                product.ImageUrl = updateProductDto.ImageUrl;
                product.CategoryId = updateProductDto.CategoryId;
                product.BrandTitle = updateProductDto.BrandTitle;

                context.SaveChanges();

                return Ok();
            }
        }

        // Удаление (изменение статуса) товара
        [HttpDelete("delete_product")]
        public IActionResult DeleteProduct([FromBody] DeleteProductDTO deleteProductDto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.Find(deleteProductDto.ProductId);
                if (product == null)
                {
                    return NotFound(new UsualProblemDetails
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    });
                }

                product.IsActive = false;
                context.SaveChanges();

                return Ok();
            }
        }
    }
}
