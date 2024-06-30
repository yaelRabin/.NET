using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class UserForResponseDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
