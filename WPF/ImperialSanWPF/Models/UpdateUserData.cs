using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    public class UpdateUserData
    {
        public int UserId { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? NewPassword { get; set; }
    }
}
