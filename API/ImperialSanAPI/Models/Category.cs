using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ImperialSanAPI.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int? ParenCategoryId { get; set; }

    public string CategoryTitle { get; set; } = null!;

    public string? CategoryDescription { get; set; }

    public virtual ICollection<Category> InverseParenCategory { get; set; } = new List<Category>();

    public virtual Category? ParenCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
