using BarberShopApi.Attributes;
using BarberShopBL.Interfaces;
using BarberShopDB.EF.Models;
using BarberShopEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopApi.Controllers
{
    [Authorize]
    [ExtractUserFromToken]
    [Controller]
    [Route("api/[controller]/[action]")]
    public class AppointmentController : Controller
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentBL _appointmentBL;
        private readonly IHttpContextAccessor _contextAccessor;

        public AppointmentController(ILogger<AppointmentController> logger,IAppointmentBL appointmentBL, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _appointmentBL = appointmentBL;
            _contextAccessor = contextAccessor;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllAppointmentsWithUsers()
        {
            try
            {
                BaseResponse<List<AppointmentWithUserDTO>> response = _appointmentBL.GetAllAppointmentsWithUsers();
                _logger.LogInformation(" GetAllAppointments process anded ");
                return StatusCode(response.StatusCode,response.Data);
            }
            catch(Exception error)
            {
                _logger.LogError($"  GetAllAppointments process failed  with error \"{error.Message}\" ");
                return StatusCode(StatusCodes.Status500InternalServerError, " GetAllAppointments process failed ");
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllAppointments()
        {
            try
            {
                BaseResponse<List<AppointmentForResponseDTO>> allAppointmentsList = _appointmentBL.GetAllAppointments();
                _logger.LogInformation(" GetAllAppointments process anded ");
                return Ok(allAppointmentsList);
            }
            catch (Exception error)
            {
                _logger.LogError($"  GetAllAppointments process failed  with error \"{error.Message}\" ");
                return StatusCode(StatusCodes.Status500InternalServerError, " GetAllAppointments process failed ");
            }
        }


        [AllowAnonymous]
        [HttpGet("{userName?}")]
        public IActionResult GetAllAppointmentsOfUser([FromRoute] string userName)
        {
            try
            {
                BaseResponse < List<AppointmentForResponseDTO> > response=_appointmentBL.GetAllAppointmentsOfUser(userName);
                return StatusCode(response.StatusCode,response.Data);

            }
            catch(Exception error)
            {
                _logger.LogError($" GetAppointmentsByUserName process failed with error : {error.Message} ");
                return StatusCode(StatusCodes.Status500InternalServerError, " GetAppointmentsByUserName process failed");

            }
        }



        [HttpPost]
        public IActionResult AddAppointment([FromBody] DateTime arrivalTime)
        {
            try
            {
                BaseResponse<AppointmentForResponseDTO> response = _appointmentBL.AddAppointment(arrivalTime);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception error)
            {
                _logger.LogError($" AddAppointment process failed with error : {error.Message} ");
                return StatusCode(StatusCodes.Status500InternalServerError, " AddAppointment process failed");
            }
        }


        [ValidateAppointmentOwner]
        [HttpPut("{id?}")]
        public IActionResult EditAppointment( [FromRoute] int id,[FromBody] DateTime newArrivalTime)
        {
            try
            {
                BaseResponse<AppointmentForResponseDTO> response=_appointmentBL.EditAppointment(id,newArrivalTime);
                return StatusCode(response.StatusCode, response);
            }
            catch(Exception error)
            {
                _logger.LogError($" EditAppointment process failed with error : {error.Message} ");
                return StatusCode(StatusCodes.Status500InternalServerError, " EditAppointment process failed");

            }
        }


        [ValidateAppointmentOwner]
        [HttpDelete]
        public IActionResult DeleteAppointment(int id)
        {
            try
            {
                BaseResponse<bool>response=_appointmentBL.DeleteAppointment(id);
                return StatusCode(response.StatusCode,response.Message);
            }
            catch(Exception error)
            {
                _logger.LogError($" DeleteAppointment process failed with error: {error.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, " DeleteAppointment failed ");
            }
        }

        //[AllowAnonymous]
        //[HttpGet("{date?}")]
        //public IActionResult GetAllAppointmentsOfDate([FromRoute]DateOnly date)
        //{
        //    try
        //    {
        //        BaseResponse<List<AppointmentForResponseDTO>> response=_appointmentBL.GetAllAppointmentsOfDate(date);
        //        return StatusCode(response.StatusCode,response);

        //    }
        //    catch(Exception error)
        //    {
        //        _logger.LogError($" GetAllAppointmentsOfDate process failed with error : {error.Message} ");
        //        return StatusCode(StatusCodes.Status500InternalServerError, " GetAllAppointmentsOfDate process failed");

        //    }
        //}

    }

    
}
