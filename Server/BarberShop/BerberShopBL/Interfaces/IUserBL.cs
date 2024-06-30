using BarberShopDB.EF.Models;
using BarberShopEntities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopBL.Interfaces
{
    public interface IUserBL
    {
        BaseResponse<UserForResponseDTO> SignUp(UserForSignUpDTO user);

        BaseResponse<UserForResponseDTO> Login(UserForLoginDTO userForLogin);

        BaseResponse<bool> LogOut();

            
    }
}
