using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public Jwt Jwt { get; set; }
        public BCrypt BCrypt { get; set; }
    }
    public class ConnectionStrings
    {
        public string BarberShop { get; set; }
    }

    public class Jwt
    {
        public string SecretKey { get; set; }
        public int ExpireMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
    public class BCrypt
    {
        public string Salt { get; set; }
    }
}
