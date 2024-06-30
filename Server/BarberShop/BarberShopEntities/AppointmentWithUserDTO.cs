using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class AppointmentWithUserDTO
    {
        public int AppointmentId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime RequestTime { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
