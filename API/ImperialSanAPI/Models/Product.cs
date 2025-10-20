using System;
using System.Collections.Generic;

namespace ImperialSanAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductTitle { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public float Price { get; set; }

    public int QuantityInStock { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public string? BrandTitle { get; set; }

    public DateOnly? DateOfCreate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BasketPosition> BasketPositions { get; set; } = new List<BasketPosition>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderPosition> OrderPositions { get; set; } = new List<OrderPosition>();
}
