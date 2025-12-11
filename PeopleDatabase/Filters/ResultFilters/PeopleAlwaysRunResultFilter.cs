using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ResultFilters
{
    public class PeopleAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Filters.OfType<SkipFilter>().Any()) 
            {
                return;
            }

            context.HttpContext.Response.Cookies.Append("Always_Run", "Always");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }
        }
    }
}
