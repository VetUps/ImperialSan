using ImperialSanAPI.DTOs.ProductDTO;
using ImperialSanAPI.Models;
using ImperialSanAPI.Utils;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImperialSanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/products
        [HttpGet]
        public ActionResult<List<CatalogProductDTO>> GetProducts()
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
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
                                                .ToList();

                return Ok(products);
            }
        }

        // GET: api/products/*id*
        [HttpGet("{id}")]
        public ActionResult<CatalogProductDTO> GetProduct(int id)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                    return NotFound();

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

        // POST api/produts
        [HttpPost]
        public ActionResult<CatalogProductDTO> Post([FromBody] AddProductDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                if (context.Products.Any(u => u.ProductTitle == dto.ProductTitle))
                    return Conflict("Название уже занят");

                if (!context.Categories.Any(c => c.CategoryId ==  dto.CategoryId))
                    return Conflict("Такой категории не существует");

                var product = new Product
                {
                    ProductId = context.Products.Count(),
                    ProductTitle = dto.ProductTitle,
                    ProductDescription = dto.ProductDescription,
                    Price = dto.Price,
                    QuantityInStock = dto.QuantityInStock,
                    ImageUrl = dto.ImageUrl,
                    CategoryId = dto.CategoryId,
                    BrandTitle = dto.BrandTitle,
                    DateOfCreate = dto.DateOfCreate,
                    IsActive = dto.IsActive,
                };

                context.Products.Add(product);
                context.SaveChanges();

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId}, product);
            }
        }

        // PUT api/products/*id*
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateProductDTO dto)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.Find(id);
                if (product == null)
                    return NotFound();

                var category = context.Categories.FirstOrDefault(c => c.CategoryId == dto.CategoryId);
                if (product == null)
                {
                    UsualProblemDetails productError = new()
                    {
                        Title = "Ошибка получения категории товара",
                        Status = StatusCodes.Status401Unauthorized,
                        Errors = new Dictionary<string, string[]>()
                        {
                               { "Category", ["Такой категории не существует"]}
                        },
                    };

                    return NotFound(productError);
                }

                product.ProductTitle = dto.ProductTitle;
                product.ProductDescription = dto.ProductDescription;
                product.Price = dto.Price;
                product.QuantityInStock = dto.QuantityInStock;
                product.ImageUrl = dto.ImageUrl;
                product.CategoryId = dto.CategoryId;
                product.BrandTitle = dto.BrandTitle;

                context.SaveChanges();

                return NoContent();
            }
        }

        // DELETE: api/products/*id*
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            using (ImperialSanContext context = new ImperialSanContext())
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                    return NotFound();

                product.IsActive = false;
                context.SaveChanges();

                return Ok();
            }
        }
    }
}
