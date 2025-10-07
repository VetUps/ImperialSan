using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImperialSanAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserMail { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string UserSurname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? UserPatronymic { get; set; }

    public string UserPhone { get; set; } = null!;

    public string? DiliveryAddress { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
