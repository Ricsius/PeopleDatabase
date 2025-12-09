using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.AuthorizationFilters
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        private const string AuthCookie = "Auth-Key";
        private const string AuthToken = "A100";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool authCookiePresent = context.HttpContext.Request.Cookies.TryGetValue(AuthCookie, out string? cookieValue);
            
            if (!authCookiePresent || cookieValue != AuthToken) 
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
    }
}
