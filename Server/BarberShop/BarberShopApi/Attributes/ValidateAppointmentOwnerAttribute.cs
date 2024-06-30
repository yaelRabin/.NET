using BarberShopDB.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BarberShopEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace BarberShopApi.Attributes
{
    public class ValidateAppointmentOwnerAttribute : ActionFilterAttribute
    {
        private ILogger<ValidateAppointmentOwnerAttribute> _logger;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;
            _logger = serviceProvider.GetService<ILogger<ValidateAppointmentOwnerAttribute>>();
            var appointmentDB = httpContext.RequestServices.GetService<IAppointmentDB>();

            int userIdFromRequest = (int)httpContext.Items["UserId"];

            if (context.ActionArguments.TryGetValue("id", out var idValue) && int.TryParse(idValue.ToString(), out int appointmentId))
            {
                var appointment = appointmentDB.GetAppointmentById(appointmentId);
                // if the appointment doesnt exist
                if (appointment == null)
                {
                    _logger.LogInformation($" user {userIdFromRequest} try to delete/edit an appointment that not exist");
                    context.Result = new ObjectResult($"Not found, appointment with id {appointmentId} does not exist.")
                    { StatusCode = 404 };
                    return;
                }

                // if the appointment is not owned by the user 
                if (appointment.UserId != userIdFromRequest)
                {
                    _logger.LogWarning($" user {userIdFromRequest} try to delete/edit an appointment that he does not own (appointmentID:{appointment.AppointmentId})");
                    context.Result = new ObjectResult($" Appointment {appointmentId} is not your own ,you are forbidden to edit or delete a session that you dont own")
                    { StatusCode = 403 };
                    //context.Result = new ForbidResult($"User {userIdFromRequest} is not allowed to modify appointment of user {appointment.UserId}.");
                    return;
                }

                // if the appointment dateTime has passed
                if (appointment.ArrivalTime < DateTime.Now)
                {
                    _logger.LogInformation($"user {userIdFromRequest} tried to delete/edit an appoinment that the dateTime has passed");
                    context.Result = new ObjectResult(" The appointment dateTime has passed, you can make another appointment ")
                    { StatusCode = 400 };
                    return;
                }

                // if the appointment date is today
                if (DateOnly.FromDateTime(appointment.ArrivalTime) ==DateOnly.FromDateTime(DateTime.Now))
                {
                    _logger.LogInformation($"user {userIdFromRequest} tried to delete/edit an appointment scheduled for today (appointmentId:{appointmentId}");
                    context.Result = new ObjectResult(" You are not allowed to change the appointment time on the day of the appointment, try calling our office ")
                    { StatusCode = 400 };
                    return;
                }

            }

            base.OnActionExecuting(context);
        }
    }
}
