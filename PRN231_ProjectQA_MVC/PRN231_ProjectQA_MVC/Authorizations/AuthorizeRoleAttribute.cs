using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PRN231_ProjectQA_MVC.Authorizations
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public AuthorizeRoleAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
              //Get role form Token
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var role = roleClaim?.Value;

                // check role
                if (role != _role)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                }
                //check time of token
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (expClaim != null)
                {
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

                    if (expTime < DateTime.UtcNow)
                    {
                        // Token has expired
                        context.Result = new RedirectToActionResult("Login", "Auth", null);
                        return;
                    }
                }

            }
            catch (SecurityTokenException)
            {
                // Token is invalid or corrupted
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }

            base.OnActionExecuting(context);
        }

    }
}
