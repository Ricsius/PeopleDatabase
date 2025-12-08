using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleDatabase.Filters.ResultFilters
{
    public class PeopleListResultFilter : IAsyncResultFilter
    {
        private ILogger<PeopleListResultFilter> _logger;

        public PeopleListResultFilter(ILogger<PeopleListResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PeopleListResultFilter), nameof(OnResultExecutionAsync));
            
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            
            await next();

            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PeopleListResultFilter), nameof(OnResultExecutionAsync));
        }
    }
}
