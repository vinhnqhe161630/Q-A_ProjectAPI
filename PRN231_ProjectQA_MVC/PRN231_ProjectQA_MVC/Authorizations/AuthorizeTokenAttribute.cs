using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace PRN231_ProjectQA_MVC.Authorizations
{
    public class AuthorizeTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                // Redirect to login if token is missing
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
            var handler = new JwtSecurityTokenHandler();
            try
            {
                //check time of token
                var jwtToken = handler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (expClaim != null)
                {
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

                    if (expTime < DateTime.UtcNow)
                    {
                        // Token has expired
                        context.Result = new RedirectToActionResult("Login", "Account", null);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Token is invalid or corrupted
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
