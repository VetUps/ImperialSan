using System;
using System.Collections.Generic;

namespace ImperialSanAPI.Models;

public partial class Basket
{
    public int BasketId { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<BasketPosition> BasketPositions { get; set; } = new List<BasketPosition>();

    public virtual User? User { get; set; }
}
