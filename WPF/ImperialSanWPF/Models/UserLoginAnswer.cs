using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Models
{
    internal class UserLoginAnswer
    {
        public int UserId { get; set; }
        public string Role { get; set; } = "User";
    }
}
