using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ImperialSanWPF.Models;

namespace ImperialSanWPF.Utils
{
    class SessionContext
    {
        public static int UserId { get; set; } = -1;
        public static string Role { get; set; } = "User";
        public static Basket UserBasket { get; set; } = new Basket();
    }
}
