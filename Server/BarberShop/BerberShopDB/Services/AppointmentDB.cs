using AutoMapper;
using BarberShopDB.EF.Contexts;
using BarberShopDB.EF.Models;
using BarberShopDB.Interfaces;
using BarberShopEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopDB.Services
{
    public class AppointmentDB : IAppointmentDB
    {
        private readonly BarberShopContext _context;
        private readonly IMapper _mapper;
        public AppointmentDB(BarberShopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<AppointmentWithUserDTO> GetAllAppointmentsWithUsers()
        {
            //var query = from appointment in _context.Appointments
            //            join user in _context.Users on appointment.UserId equals user.UserId
            //            select new AppointmentWithUserDTO
            //            {
            //                AppointmentId = appointment.AppointmentId,
            //                ArrivalTime = appointment.ArrivalTime,
            //                RequestTime = appointment.RequestTime,
            //                UserId = appointment.UserId,
            //                Name = user.Name,
            //                Phone = user.Phone
            //            };
            //return query.ToList();

            var query = _context.Appointments
                .Include(a => a.User)
                .Select(a => new AppointmentWithUserDTO
                {
                    AppointmentId = a.AppointmentId,
                    ArrivalTime = a.ArrivalTime,
                    RequestTime = a.RequestTime,
                    UserId = a.UserId,
                    Name = a.User.Name,
                    Phone = a.User.Phone,
                });
            return query.ToList();

        }

        public List<Appointment> GetAllAppointments()
        {
            return _context.Appointments.OrderBy(a=>a.ArrivalTime).ToList();
        }
        public List<Appointment> GetAllAppointmentsOfUser(string userName)
        {
            return _context.Appointments
            .Include(a => a.User)
            .Where(a => a.User.UserName == userName)
            .OrderBy(a=>a.ArrivalTime)
            .ToList();

        }
        public List<Appointment> GetAllAppointmentsOfDate(DateOnly date)
        {
            return _context.Appointments
                .Where(a =>DateOnly.FromDateTime(a.ArrivalTime) == date)
                .ToList();
        }


        public bool IsAvailableDateTime(DateTime dateTime)
        {
            // אם יותר תור שמתחיל או מסתיים בטווח התור החדש (מתחילת התור 30 דקות קדימה) אז התור החדש חופף לתור קיים-אי אפשר
            DateTime appointmentEndTime = dateTime.AddMinutes(30);

            return !_context.Appointments
                .Any(a =>
               (a.ArrivalTime >= dateTime && a.ArrivalTime < appointmentEndTime)
                   ||
              (a.ArrivalTime.AddMinutes(30) > dateTime && a.ArrivalTime.AddMinutes(30) < appointmentEndTime)
                );

        }

        public Appointment GetAppointmentById(int id)
        {
            return _context
                 .Appointments
                 .AsNoTracking()
                 .FirstOrDefault(appoint => appoint.AppointmentId == id);
        }

        public Appointment AddAppointment(Appointment newAppointment)
        {
            _context
                .Appointments
                .Add(newAppointment);
            _context.SaveChanges();
            return newAppointment;
        }

        public Appointment EditAppointment(int id, DateTime newArrivalTime)
        {

            Appointment appoint = _context
                .Appointments
                .FirstOrDefault(app => app.AppointmentId == id);
            appoint.ArrivalTime = newArrivalTime;
            _context.SaveChanges();
            return appoint;

        }
        public void DeleteAppointment(Appointment appointmentRemove)
        {
            _context.Appointments.Remove(appointmentRemove);
            _context.SaveChanges();
        }

    }
}
