using System;
using System.Collections.Generic;

namespace ImperialSanAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateOnly? DateOfCreate { get; set; }

    public string? OrderStatus { get; set; }

    public string DiliveryAddres { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public float Price { get; set; }

    public string? UserComment { get; set; }

    public virtual ICollection<OrderPosition> OrderPositions { get; set; } = new List<OrderPosition>();

    public virtual User? User { get; set; }
}
