using System.Numerics;
using ImperialSanAPI.DTOs.ProductDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // Получение всех товаров
        [HttpGet]
        public ActionResult<CatalogProductPaginationDTO> GetProducts(int pageNumber, int pageSize)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (pageNumber <=0)
                {
                    UsualProblemDetails paginationError = new()
                    {
                        Title = "Ошибка пагинации",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "PageNumber", ["Такой страницы не существует"]}
                        },
                    };

                    return NotFound(paginationError);
                }

                if (pageSize <=0)
                {
                    UsualProblemDetails paginationError = new()
                    {
                        Title = "Ошибка пагинации",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "PageSize", ["Размер страницы недопустим"]}
                        },
                    };

                    return NotFound(paginationError);
                }

                var products = context.Products.Select(p => new CatalogProductDTO
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
                                                .OrderBy(p => p.ProductId)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

                int totalCount = context.Products.Count();

                CatalogProductPaginationDTO catalogProductPagination = new CatalogProductPaginationDTO
                {
                    Products = products,
                    TotalProductsCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(catalogProductPagination);
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
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    };

                    return NotFound(productError);
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
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Товар с таким названием у этого бренда уже есть"]}
                        },
                    };

                    return Conflict(productError);
                }

                if (!context.Categories.Any(c => c.CategoryId ==  addProductDto.CategoryId))
                {
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения кагетории",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                    };

                    return Conflict(productError);
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
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    };

                    return NotFound(productError);
                }

                if (context.Products.Where(p => p.BrandTitle == updateProductDto.BrandTitle && p.ProductId != updateProductDto.ProductId)
                                    .Any(u => u.ProductTitle == updateProductDto.ProductTitle))
                {
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Товар с таким названием у этого бренда уже есть"]}
                        },
                    };

                    return Conflict(productError);
                }

                if (!context.Categories.Any(c => c.CategoryId == updateProductDto.CategoryId))
                {
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения кагетории",
                        Status = StatusCodes.Status409Conflict,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                    };

                    return Conflict(productError);
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
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения товара",
                        Status = StatusCodes.Status404NotFound,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Product", ["Такого товара не существует"]}
                        },
                    };

                    return NotFound(productError);
                }

                product.IsActive = false;
                context.SaveChanges();

                return Ok();
            }
        }
    }
}
