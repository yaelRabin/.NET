using BarberShopDB.EF.Models;
using BarberShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopDB.Interfaces
{
    public interface IAppointmentDB
    {
        List<AppointmentWithUserDTO> GetAllAppointmentsWithUsers();
        List<Appointment> GetAllAppointments();
        List<Appointment>GetAllAppointmentsOfUser(string userName);
        List<Appointment>GetAllAppointmentsOfDate(DateOnly date);
        bool IsAvailableDateTime(DateTime dateTime);
        Appointment GetAppointmentById(int id);
        Appointment AddAppointment(Appointment newAppointment);

        Appointment EditAppointment(int id, DateTime newArrivalTime);
        void DeleteAppointment(Appointment appointment);
    }
}
