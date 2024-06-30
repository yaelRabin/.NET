using AutoMapper;
using BarberShopBL.Interfaces;
using BarberShopDB.EF.Models;
using BarberShopDB.Interfaces;
using BarberShopEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopBL.Services
{
    public class AppointmentBL : IAppointmentBL
    {
        private readonly IAppointmentDB _appointmentDB;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserDB _userDB;
        private readonly ILogger<AppointmentBL> _logger;
        private readonly IMapper _mapper;
        public AppointmentBL(IAppointmentDB appointmentDB, IHttpContextAccessor httpContextAccessor, IUserDB userDB,ILogger<AppointmentBL> logger,IMapper mapper)
        {
            _appointmentDB = appointmentDB;
            _httpContextAccessor = httpContextAccessor;
            _userDB = userDB;
            _logger = logger;
            _mapper = mapper;
        }

        public BaseResponse< List<AppointmentWithUserDTO>> GetAllAppointmentsWithUsers()
        {
           List<AppointmentWithUserDTO> listForResponse = _appointmentDB.GetAllAppointmentsWithUsers();
            return new BaseResponse<List<AppointmentWithUserDTO>>(listForResponse,200,"list of all appointments with users");
        }
        public BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointments()
        {
            List<Appointment> listFromDB=_appointmentDB.GetAllAppointments().ToList();
            List<AppointmentForResponseDTO>listForResponse=new List<AppointmentForResponseDTO>();
            foreach (Appointment appointment in listFromDB)
            {
                listForResponse.Add(_mapper.Map<AppointmentForResponseDTO>(appointment));
            }
            return new BaseResponse<List<AppointmentForResponseDTO>>(listForResponse,200,"list of all appoinments");
        }

        public BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointmentsOfUser(string userName)
        {
            User user=_userDB.GetUserByUserName(userName);
            if(user == null)
            {
                _logger.LogInformation($"anonimos user tried to get all the appointments for user that not exist");
                return new BaseResponse<List<AppointmentForResponseDTO>>(null, 404, "Not found,user doesnt exist");
            }
            List<Appointment> listFromDB = _appointmentDB.GetAllAppointmentsOfUser(userName);
            List<AppointmentForResponseDTO> listForResponse = new List<AppointmentForResponseDTO>();
            foreach (Appointment appointment in listFromDB)
            {
                listForResponse.Add(_mapper.Map<AppointmentForResponseDTO>(appointment));
            }
            return new BaseResponse<List<AppointmentForResponseDTO>>(listForResponse,200,$"appointments for user {userName}");

        }
        public BaseResponse<List<AppointmentForResponseDTO>> GetAllAppointmentsOfDate(DateOnly date)
        {
            List<Appointment> listFromDB=_appointmentDB.GetAllAppointmentsOfDate(date);
            List<AppointmentForResponseDTO> listForResponse=new List<AppointmentForResponseDTO>();
            foreach (Appointment appointment in listFromDB)
            {
                listForResponse.Add(_mapper.Map<Appointment, AppointmentForResponseDTO>(appointment));
            }
            return new BaseResponse<List<AppointmentForResponseDTO>>(listForResponse, 200, $"list of all appointments for date {date}");
        }


        private bool IsWithinOperatingHours(DateTime arrivalTime)
        {
            TimeSpan openingTime = new TimeSpan(11, 0, 0); // 11 AM
            TimeSpan closingTime = new TimeSpan(20, 0, 0); // 8 PM
            TimeSpan lastAppointmentTime = closingTime - TimeSpan.FromMinutes(30); // 30 minutes before closing

            TimeSpan appointmentTime = arrivalTime.TimeOfDay;

            return appointmentTime >= openingTime && appointmentTime <= lastAppointmentTime;
        }
        
        private bool IsOpenDay(DateTime arrivalTime)
        {
            // Check if the day is Friday (5) or Saturday (6)
            return !(arrivalTime.DayOfWeek == DayOfWeek.Friday || arrivalTime.DayOfWeek == DayOfWeek.Saturday);
        }

        public BaseResponse<AppointmentForResponseDTO> AddAppointment(DateTime arrivalTime)
        {
            //get the id and userName from the token
            int userIdFromRequest = (int)_httpContextAccessor.HttpContext.Items["UserId"];
            string userName = (string)_httpContextAccessor.HttpContext.Items["UserName"];
            // check if there is no conflict with the arrival time
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly arrivalDate = DateOnly.FromDateTime(arrivalTime);
            if(arrivalTime<=DateTime.Now||currentDate==arrivalDate)
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to invalid dateTime");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " invalid DateTime, you can schedule an appointment only to DateTime greater than today ");
            }
            if (!IsOpenDay(arrivalTime))
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to close day");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " the barber-shop is not active on the requested day");
            }
            if(!IsWithinOperatingHours(arrivalTime))
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to close time");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " the barber-shop is not active at the requested time");
            }
            if(!_appointmentDB.IsAvailableDateTime(arrivalTime)) 
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant for unavailable dateTime");
                return new BaseResponse<AppointmentForResponseDTO>(null, 409, " sorry, the dateTime is unavailable, there is an appointment in this time");

            }
            Appointment appointmentToAdd = new Appointment()
            {
                ArrivalTime = arrivalTime,
                RequestTime = DateTime.Now,
                UserId = userIdFromRequest,
            };
            Appointment newAppointment = _appointmentDB.AddAppointment(appointmentToAdd);
            AppointmentForResponseDTO resAppointment = _mapper.Map<AppointmentForResponseDTO>(newAppointment);
            _logger.LogInformation($" user {userIdFromRequest} added an appointment to {appointmentToAdd.ArrivalTime}");
            return new BaseResponse<AppointmentForResponseDTO>(resAppointment, 200, " addAppointment process anded");

        }
        
        
        public BaseResponse<AppointmentForResponseDTO> EditAppointment(int id, DateTime newArrivalTime)
        {
            int userIdFromRequest = (int)_httpContextAccessor.HttpContext.Items["UserId"];
            string userName = (string)_httpContextAccessor.HttpContext.Items["UserName"];
            Appointment appointmentToEdit = _appointmentDB.GetAppointmentById(id); //שליפת פרטי התור המבוקש
            if (newArrivalTime <= DateTime.Now)
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to invalid dateTime");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " invalid DateTime, insert DateTime greater than today  ");
            }
            if (!IsOpenDay(newArrivalTime))
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to close day");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " the barber-shop is not active on the requested day");
            }
            if (!IsWithinOperatingHours(newArrivalTime))
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant to close time");
                return new BaseResponse<AppointmentForResponseDTO>(null, 400, " the barber-shop is not active at the requested time");
            }
            if (!_appointmentDB.IsAvailableDateTime(newArrivalTime))
            {
                _logger.LogError($"user {userIdFromRequest} tried to schedule an appointmant for unavailable dateTime");
                return new BaseResponse<AppointmentForResponseDTO>(null, 409, " sorry, the dateTime is unavailable, there is an appointment in this time");

            }
            Appointment editedAppointment = _appointmentDB.EditAppointment( appointmentToEdit.AppointmentId, newArrivalTime); // קריאה לפונקציה משכבת הדטהבייס שמעדכנת את התור
            _logger.LogInformation($"user {userIdFromRequest} edited appoinment (last-arrivalTime:{appointmentToEdit.ArrivalTime}  new arrivalTime:{editedAppointment.ArrivalTime}");
            return new BaseResponse<AppointmentForResponseDTO>(_mapper.Map<AppointmentForResponseDTO>(editedAppointment), 200, $" appointment edited to new arrivalTime: {newArrivalTime}");

        }


        public BaseResponse<bool> DeleteAppointment(int id)
        {
            int userIdFromRequest = (int)_httpContextAccessor.HttpContext.Items["UserId"];
            string userName = (string)_httpContextAccessor.HttpContext.Items["UserName"];
            Appointment appointmentToDelete = _appointmentDB.GetAppointmentById(id); //שליפת פרטי התור המבוקש
            _appointmentDB.DeleteAppointment(appointmentToDelete);
            _logger.LogInformation($"user {userIdFromRequest} delete appointment (arrivalTime: {appointmentToDelete.ArrivalTime})");
            return new BaseResponse<bool>(true, 200, " appointment deleted ");

        }


    }
}
