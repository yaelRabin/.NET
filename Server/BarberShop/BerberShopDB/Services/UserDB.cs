using AutoMapper;
using BarberShopDB.EF.Contexts;
using BarberShopDB.EF.Models;
using BarberShopDB.Interfaces;
using BarberShopEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopDB.Services
{
    public class UserDB : IUserDB
    {
        private readonly BarberShopContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        public UserDB(BarberShopContext context,IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }


        //פונקציה שמחזירה את המשתמש בעל שם המשתמש שהתקבל- אם לא קיים מחזירה נאל
        public User GetUserByUserName(string userName)
        {
            return _context
                .Users
                .AsNoTracking()
                .FirstOrDefault(user => user.UserName == userName);
        }

        public User GetUserById(int id)
        {
            return _context
                .Users
                .AsNoTracking()
                .FirstOrDefault(user => user.UserId == id);
        }



        //פונקציה שמקבלת שם משתמש וטלפון ובודקת האם יש בדטהבייס יוזר עם שם משתמש זהה או טלפון זהה
        public User IsUserNameOrPhoneExist(string userName,string phone)
        {
            User user =
                _context
                .Users
                .AsNoTracking()
                .FirstOrDefault(user => user.UserName == userName||user.Phone==phone);
            return user;
        }


        // פונקציה שמקבלת יוזר ומוסיפה אותו לדטהבייס
        public User AddUserToDB(User user)
        {
            _context.Users.Add(user);
           _context.SaveChanges();
            return user;
        }
    }
}
