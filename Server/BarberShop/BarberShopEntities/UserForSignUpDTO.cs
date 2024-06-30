using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class UserForSignUpDTO
    {
        public string UserName { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
