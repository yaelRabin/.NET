using BarberShopEntities;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BarberShopApi.Attributes
{
    public class ExtractUserFromTokenAttribute : ActionFilterAttribute
    {

        // attribute that extract the userId and userName from the token in cookies and save it in httpContext.Items
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var stringToken = httpContext.Request.Cookies[CookiesKeys.AccessToken];
            if (!string.IsNullOrEmpty(stringToken))
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(stringToken);
                int userId = int.Parse(token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
                string userName = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

                httpContext.Items["UserId"] = userId;
                httpContext.Items["UserName"] = userName;
            }
            base.OnActionExecuting(context);
        }
    }
}
