using System;
using System.Collections.Generic;

namespace BarberShopDB.EF.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public DateTime ArrivalTime { get; set; }

    public int UserId { get; set; }

    public DateTime RequestTime { get; set; }

    public virtual User User { get; set; } = null!;
}
