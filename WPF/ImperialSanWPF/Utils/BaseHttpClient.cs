using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Utils
{
    internal class BaseHttpClient
    {
        public static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:3000/api/"),
        };
    }
}
