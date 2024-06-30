using BarberShopBL.Interfaces;
using BarberShopDB.EF.Models;
using BarberShopEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BarberShopApi.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SignUp([FromBody] UserForSignUpDTO newUser)
        {
            try
            {
                BaseResponse<UserForResponseDTO> result = _userBL.SignUp(newUser);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception error)
            {
                _logger.LogError($" failed in SignUp process. \n Message: {error.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, " SignUp process failed");
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {
            try
            {
                BaseResponse<UserForResponseDTO> response = _userBL.Login(userForLogin);
                return StatusCode(response.StatusCode,response);
            }
            catch(Exception error)
            {
                _logger.LogError($"failed in Login process .\n message: {error.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError," Login process failed");
            }
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            try
            {
                BaseResponse<bool> result = _userBL.LogOut();
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception error)
            {
                _logger.LogError($" failed in LogOut process ");
                return StatusCode(StatusCodes.Status500InternalServerError," Logout process failed" );
            }
        }

    }
}
