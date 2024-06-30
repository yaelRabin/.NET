using AutoMapper;
using BarberShopDB.EF.Models;
using BarberShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopBL
{
    public class MapperManager : Profile
    {
        public MapperManager()
        {
            CreateMap<UserForSignUpDTO, User>();
            CreateMap<User, UserForResponseDTO>().ForMember(resUser => resUser.Name,options=>options.MapFrom(user=>user.UserName));
            CreateMap<Appointment, AppointmentForResponseDTO>();
        }
    }
}
