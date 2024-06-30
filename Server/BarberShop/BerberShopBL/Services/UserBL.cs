using AutoMapper;
using BarberShopBL.Interfaces;
using BarberShopDB.EF.Models;
using BarberShopDB.Interfaces;
using BarberShopEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopBL.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserDB _userDB;
        private readonly ILogger<UserBL> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public UserBL(IUserDB userDB, ILogger<UserBL> logger, IOptions<AppSettings> appSettings,IHttpContextAccessor httpContextAccessor,IMapper mapper)
        {
            _userDB = userDB;
            _logger = logger;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        

        public BaseResponse<UserForResponseDTO> SignUp(UserForSignUpDTO userForSignUp)
        {
            User sameUser = _userDB.IsUserNameOrPhoneExist(userForSignUp.UserName, userForSignUp.Phone); // check if there is user with such userName or phone by searching in the dataBase
            if (sameUser != null)
            {
                if (sameUser.UserName == userForSignUp.UserName) // if there is user with such userName
                {
                    _logger.LogWarning($" SignUp process failed with statusCode 409 conflict - userName exist in the system ");
                    return new BaseResponse<UserForResponseDTO>(null, 409, " conflict - there is user with such userName");
                }
                if (sameUser.Phone == userForSignUp.Phone)// if there is user with such phone
                {
                    _logger.LogWarning($" SignUp process failed with statusCode 409 conflict - Phone exist in the system ");
                    return new BaseResponse<UserForResponseDTO>(null, 409, " conflict - there is user with such Phone");
                }
            }
            userForSignUp.UserPassword = BCrypt.Net.BCrypt.HashPassword(userForSignUp.UserPassword); // hash the password
            User newUser = _mapper.Map<User>(userForSignUp); // convert from UserForSignUpDTO type to User type
            newUser = _userDB.AddUserToDB(newUser);// add the user to the dataBase ( by using hte function from DB layer)
            CreateCookieWithToken(newUser); // create cookie and token into the cookie
            _logger.LogInformation($" user {newUser.UserId} joined to the system");
            return new BaseResponse<UserForResponseDTO>(_mapper.Map<UserForResponseDTO>(newUser), 200, " signUp process ended");
        }

        public BaseResponse<UserForResponseDTO> Login(UserForLoginDTO userForLogin)
        {
            User foundUser = _userDB.GetUserByUserName(userForLogin.UserName); // find in the dataBase user with such userName
            if (foundUser == null) // if the foundUser is null - not found in the dataBase user with such userName 
            {
                _logger.LogWarning($" failed to log in with UserName {userForLogin.UserName},  user does not exist ");
                return new BaseResponse<UserForResponseDTO>(null, 404, "sorry, user not found in our system, please sign-up");
            }
            bool verifyPassword= BCrypt.Net.BCrypt.Verify(userForLogin.UserPassword, foundUser.UserPassword);// check if the password from the client is equal to the hashed password from the dataBase
            if (!verifyPassword) // if the client send wrong password - error 401,Unauthorized
            {
                _logger.LogWarning($" user {userForLogin.UserName} try to login with wrong password ");
                return new BaseResponse<UserForResponseDTO>(null, 401, "Unauthorized, invalid password ");
            }
            _logger.LogInformation($" User {foundUser.UserId} has logged in");
            CreateCookieWithToken(foundUser);// create cookie and token into the cookie
            UserForResponseDTO resUser = _mapper.Map<UserForResponseDTO>(foundUser);// convert User type to UserForResponseDTO type (security- the client cant get the entire record) 
            return new BaseResponse<UserForResponseDTO>(resUser, 200, "connected user");
        }

        public BaseResponse<bool> LogOut()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CookiesKeys.AccessToken);
            _logger.LogInformation($"user logout process");
            return new BaseResponse<bool>(true,200,"logout ended successfully");
        }

        public string GenerateAccessToken(User user)
        {
            var jwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.SecretKey));
            var credetials = new SigningCredentials(jwtSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
            };
            var token = new JwtSecurityToken(
                    _appSettings.Jwt.Issuer,
                    _appSettings.Jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(_appSettings.Jwt.ExpireMinutes),
                    signingCredentials: credetials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public void CreateCookieWithToken(User user)
        {
            string token=GenerateAccessToken(user);
            CookieOptions cookieTokenOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(_appSettings.Jwt.ExpireMinutes)

            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CookiesKeys.AccessToken,token,cookieTokenOptions);
        }



    }
}




//byte[] salt;
//const int keySize = 64;
//const int iterations = 350000;
//HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
//public string ToHashPasword(string password, out byte[] salt)
//{
//    salt = RandomNumberGenerator.GetBytes(keySize);
//    var hash = Rfc2898DeriveBytes.Pbkdf2(
//        Encoding.UTF8.GetBytes(password),
//        salt,
//        iterations,
//        hashAlgorithm,
//        keySize);
//    return Convert.ToHexString(hash);
//}

