using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ResultFilters
{
    public class PeopleAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            context.HttpContext.Response.Cookies.Append("Always_Run", "Always");
        }
    }
}
