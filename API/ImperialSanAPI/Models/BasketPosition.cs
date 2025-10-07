using System;
using System.Collections.Generic;

namespace ImperialSanAPI.Models;

public partial class BasketPosition
{
    public int BasketPositionId { get; set; }

    public int? BasketId { get; set; }

    public int? ProductId { get; set; }

    public int? ProductQuantity { get; set; }

    public virtual Basket? Basket { get; set; }

    public virtual Product? Product { get; set; }
}
