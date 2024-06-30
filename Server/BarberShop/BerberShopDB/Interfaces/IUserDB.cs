using BarberShopDB.EF.Models;
using BarberShopEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopDB.Interfaces
{
    public interface IUserDB
    {
        User GetUserByUserName(string userName);
        User GetUserById(int id);
        User IsUserNameOrPhoneExist(string userName, string phone);
        User AddUserToDB(User user);
    }
}
