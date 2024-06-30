using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopEntities
{
    public class AppointmentForResponseDTO
    {
        public int AppointmentId { get; set; }

        public DateTime ArrivalTime { get; set; }

        public int UserId { get; set; }

        public DateTime RequestTime { get; set; }

    }
}
