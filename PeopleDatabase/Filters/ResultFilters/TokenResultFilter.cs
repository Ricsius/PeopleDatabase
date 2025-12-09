using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ResultFilters
{
    public class TokenResultFilter : IResultFilter
    {
        private const string AuthCookie = "Auth-Key";
        private const string AuthToken = "A100";

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append(AuthCookie, AuthToken);
        }
    }
}
