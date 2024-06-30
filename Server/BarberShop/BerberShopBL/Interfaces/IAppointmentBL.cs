using BarberShopDB.EF.Models;
using BarberShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopBL.Interfaces
{
    public interface IAppointmentBL
    {
        BaseResponse<List<AppointmentWithUserDTO>> GetAllAppointmentsWithUsers();
        BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointments();
        BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointmentsOfUser(string userName);
        BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointmentsOfDate(DateOnly date);
        BaseResponse<AppointmentForResponseDTO> AddAppointment(DateTime arrivalTime);

        BaseResponse<AppointmentForResponseDTO> EditAppointment(int id, DateTime newArrivalTime);
        BaseResponse<bool> DeleteAppointment(int id);
    }
}
