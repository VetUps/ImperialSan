using System;
using System.Collections.Generic;

namespace ImperialSanAPI.Models;

public partial class OrderPosition
{
    public int OrderPositionId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? ProductQuantity { get; set; }

    public float ProductPriceInMoment { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
